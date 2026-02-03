package main

import (
	"io"
	"log"
	"net"
	"os"
	"strings"
	"sync"
)

func main() {
	listenAddr := getEnvString("LISTEN_ADDR", "0.0.0.0:25565")
	upstreamHost := getEnvString("UPSTREAM_HOST", "mc-server")
	upstreamPort := getEnvString("UPSTREAM_PORT", "25565")
	upstreamAddr := upstreamHost + ":" + upstreamPort

	log.Printf("Starting TCP proxy on %s -> %s", listenAddr, upstreamAddr)

	listener, err := net.Listen("tcp", listenAddr)
	if err != nil {
		log.Fatalf("Failed to listen on %s: %v", listenAddr, err)
	}

	defer listener.Close()

	for {
		clientConnection, err := listener.Accept()
		if err != nil {
			log.Printf("Failed to accept connection: %v", err)
			continue
		}

		go handleConnection(clientConnection, upstreamAddr)
	}
}

func handleConnection(clientConnection net.Conn, upstreamAddr string) {
	defer clientConnection.Close()

	upstreamConnection, err := net.Dial("tcp", upstreamAddr)
	if err != nil {
		log.Printf("Failed to connect to upstream %s: %v", upstreamAddr, err)
		return
	}

	defer upstreamConnection.Close()

	var waitGroup sync.WaitGroup
	waitGroup.Add(2)

	go func() {
		defer waitGroup.Done()
		_, _ = io.Copy(upstreamConnection, clientConnection)
		if tcpConnection, ok := upstreamConnection.(*net.TCPConn); ok {
			_ = tcpConnection.CloseWrite()
		}
	}()

	go func() {
		defer waitGroup.Done()
		_, _ = io.Copy(clientConnection, upstreamConnection)
		if tcpConnection, ok := clientConnection.(*net.TCPConn); ok {
			_ = tcpConnection.CloseWrite()
		}
	}()

	waitGroup.Wait()
}

func getEnvString(name string, defaultValue string) string {
	value := strings.TrimSpace(os.Getenv(name))
	if value == "" {
		return defaultValue
	}

	return value
}
