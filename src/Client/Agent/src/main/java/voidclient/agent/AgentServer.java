package voidclient.agent;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;

final class AgentServer implements Runnable {
    private final ServerSocket serverSocket;
    private final String token;

    AgentServer(ServerSocket serverSocket, String token) {
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
            String request = reader.readLine();
            String[] parts = request == null ? new String[0] : request.split("\\t", 3);
            String response;

            if (parts.length == 0 || !token.equals(parts[0]))
                response = "{\"status\":\"unavailable\",\"message\":\"Invalid tracker token\"}";
            else if (parts.length == 1 || (parts.length == 2 && "snapshot".equals(parts[1])))
                response = Tracker.snapshotJson();
            else if (parts.length == 3 && "connect".equals(parts[1]))
                response = connect(parts[2]);
            else
                response = "{\"status\":\"unavailable\",\"message\":\"Unknown agent command\"}";

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

    private String connect(String encodedAddress) {
        try {
            String address = new String(Base64.getUrlDecoder().decode(encodedAddress), StandardCharsets.UTF_8);

            if (address.isEmpty())
                return "{\"status\":\"unavailable\",\"stage\":\"request.validation\",\"message\":\"The server address is empty\"}";

            return DirectConnectController.connectJson(address);
        } catch (IllegalArgumentException exception) {
            return "{\"status\":\"unavailable\",\"stage\":\"request.validation\",\"message\":\"The server address is not valid Base64URL\"}";
        }
    }
}
