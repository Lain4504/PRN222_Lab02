// SignalR Base Client
class SignalRClient {
    constructor() {
        this.connections = {};
        this.isConnected = false;
    }

    // Initialize all SignalR connections
    async initializeConnections() {
        try {
            // News Article Hub
            this.connections.newsArticle = new signalR.HubConnectionBuilder()
                .withUrl("/newsArticleHub")
                .build();

            // Category Hub
            this.connections.category = new signalR.HubConnectionBuilder()
                .withUrl("/categoryHub")
                .build();

            // System Account Hub
            this.connections.systemAccount = new signalR.HubConnectionBuilder()
                .withUrl("/systemAccountHub")
                .build();

            // Start all connections
            await Promise.all([
                this.connections.newsArticle.start(),
                this.connections.category.start(),
                this.connections.systemAccount.start()
            ]);

            this.isConnected = true;
            console.log("All SignalR connections established successfully");

            // Setup connection error handlers
            this.setupErrorHandlers();

            // Join appropriate groups
            await this.joinGroups();

        } catch (error) {
            console.error("Error establishing SignalR connections:", error);
            this.isConnected = false;
        }
    }

    setupErrorHandlers() {
        Object.keys(this.connections).forEach(key => {
            const connection = this.connections[key];
            
            connection.onclose(async () => {
                console.log(`${key} connection closed. Attempting to reconnect...`);
                this.isConnected = false;
                await this.reconnect(key);
            });

            connection.onreconnecting(() => {
                console.log(`${key} connection lost. Reconnecting...`);
                this.isConnected = false;
            });

            connection.onreconnected(() => {
                console.log(`${key} connection reestablished`);
                this.isConnected = true;
            });
        });
    }

    async reconnect(connectionKey) {
        try {
            await this.connections[connectionKey].start();
            console.log(`${connectionKey} reconnected successfully`);
            this.isConnected = true;
        } catch (error) {
            console.error(`Failed to reconnect ${connectionKey}:`, error);
            // Retry after 5 seconds
            setTimeout(() => this.reconnect(connectionKey), 5000);
        }
    }

    async joinGroups() {
        try {
            // Join news article group
            await this.connections.newsArticle.invoke("JoinNewsArticleGroup");
            
            // Join category group (admin only)
            await this.connections.category.invoke("JoinCategoryGroup");
            
            // Join account group (admin only)
            await this.connections.systemAccount.invoke("JoinAccountGroup");
            
            console.log("Joined SignalR groups successfully");
        } catch (error) {
            console.error("Error joining SignalR groups:", error);
        }
    }

    // Get specific connection
    getConnection(hubName) {
        return this.connections[hubName];
    }

    // Check if connected
    isConnectionActive() {
        return this.isConnected;
    }

    // Disconnect all connections
    async disconnect() {
        try {
            await Promise.all(
                Object.values(this.connections).map(connection => connection.stop())
            );
            this.isConnected = false;
            console.log("All SignalR connections closed");
        } catch (error) {
            console.error("Error closing SignalR connections:", error);
        }
    }
}

// Global SignalR client instance
window.signalRClient = new SignalRClient();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', async function() {
    await window.signalRClient.initializeConnections();
});

// Cleanup on page unload
window.addEventListener('beforeunload', async function() {
    await window.signalRClient.disconnect();
});
