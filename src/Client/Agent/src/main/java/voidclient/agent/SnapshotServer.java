package voidclient.agent;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

final class SnapshotServer implements Runnable {
    private final ServerSocket serverSocket;
    private final String token;

    SnapshotServer(ServerSocket serverSocket, String token) {
        this.serverSocket = serverSocket;
        this.token = token;
    }

    @Override
    public void run() {
        while (!serverSocket.isClosed()) {
            try {
                serve(serverSocket.accept());
            } catch (IOException exception) {
                if (!serverSocket.isClosed())
                    System.err.println("Void client state agent request failed: " + exception.getMessage());
            }
        }
    }

    private void serve(Socket socket) {
        try {
            socket.setSoTimeout(2000);
            BufferedReader reader = new BufferedReader(new InputStreamReader(socket.getInputStream(), StandardCharsets.UTF_8));
            BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream(), StandardCharsets.UTF_8));
            String requestedToken = reader.readLine();

            String response = token.equals(requestedToken)
                ? Tracker.snapshotJson()
                : "{\"status\":\"unavailable\",\"message\":\"Invalid tracker token\"}";

            writer.write(response);

            writer.newLine();
            writer.flush();
        } catch (IOException exception) {
            System.err.println("Void client state agent response failed: " + exception.getMessage());
        } finally {
            try {
                socket.close();
            } catch (IOException exception) {
                // The request is already complete.
            }
        }
    }
}
