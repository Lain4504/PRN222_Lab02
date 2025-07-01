// System Account Real-time functionality
class AccountRealTime {
    constructor() {
        this.connection = null;
        this.isInitialized = false;
    }

    async initialize() {
        if (!window.signalRClient || !window.signalRClient.isConnectionActive()) {
            console.log("Waiting for SignalR connection...");
            setTimeout(() => this.initialize(), 1000);
            return;
        }

        this.connection = window.signalRClient.getConnection('systemAccount');
        this.setupEventHandlers();
        this.isInitialized = true;
        console.log("Account real-time initialized");
    }

    setupEventHandlers() {
        // Account Created
        this.connection.on("AccountCreated", (account) => {
            this.handleAccountCreated(account);
        });

        // Account Updated
        this.connection.on("AccountUpdated", (account) => {
            this.handleAccountUpdated(account);
        });

        // Account Deleted
        this.connection.on("AccountDeleted", (accountId) => {
            this.handleAccountDeleted(accountId);
        });

        // Profile Updated (for current user)
        this.connection.on("ProfileUpdated", (account) => {
            this.handleProfileUpdated(account);
        });

        // Force Logout
        this.connection.on("ForceLogout", (message) => {
            this.handleForceLogout(message);
        });

        // User Login/Logout notifications
        this.connection.on("UserLoggedIn", (accountId, accountName) => {
            this.handleUserLoggedIn(accountId, accountName);
        });

        this.connection.on("UserLoggedOut", (accountId, accountName) => {
            this.handleUserLoggedOut(accountId, accountName);
        });
    }

    handleAccountCreated(account) {
        console.log("New account created:", account);
        
        // Show notification
        this.showNotification(`New account created: ${account.accountName}`, 'success');
        
        // Add to table if on index page
        this.addAccountToTable(account);
        
        // Update counters
        this.updateCounters();
    }

    handleAccountUpdated(account) {
        console.log("Account updated:", account);
        
        // Show notification
        this.showNotification(`Account updated: ${account.accountName}`, 'info');
        
        // Update table row if exists
        this.updateAccountInTable(account);
        
        // Update details if on details page
        this.updateAccountDetails(account);
    }

    handleAccountDeleted(accountId) {
        console.log("Account deleted:", accountId);
        
        // Show notification
        this.showNotification('Account has been deleted', 'warning');
        
        // Remove from table
        this.removeAccountFromTable(accountId);
        
        // Redirect if on deleted account's page
        if (window.location.pathname.includes(accountId)) {
            window.location.href = '/Account';
        }
        
        // Update counters
        this.updateCounters();
    }

    handleProfileUpdated(account) {
        console.log("Profile updated:", account);
        
        // Show notification
        this.showNotification('Your profile has been updated', 'info');
        
        // Update profile information on current page
        this.updateCurrentUserProfile(account);
    }

    handleForceLogout(message) {
        console.log("Force logout:", message);
        
        // Show alert
        alert(message);
        
        // Redirect to login
        window.location.href = '/Account/Login';
    }

    handleUserLoggedIn(accountId, accountName) {
        console.log("User logged in:", accountName);
        
        // Show notification to admins
        this.showNotification(`${accountName} logged in`, 'info');
        
        // Update online status if showing user list
        this.updateUserOnlineStatus(accountId, true);
    }

    handleUserLoggedOut(accountId, accountName) {
        console.log("User logged out:", accountName);
        
        // Show notification to admins
        this.showNotification(`${accountName} logged out`, 'info');
        
        // Update online status if showing user list
        this.updateUserOnlineStatus(accountId, false);
    }

    addAccountToTable(account) {
        const tableBody = document.querySelector('#accountsTable tbody');
        if (!tableBody) return;

        const row = this.createTableRow(account);
        tableBody.insertBefore(row, tableBody.firstChild);
        
        // Highlight new row
        row.classList.add('table-success');
        setTimeout(() => row.classList.remove('table-success'), 3000);
    }

    updateAccountInTable(account) {
        const row = document.querySelector(`tr[data-account-id="${account.accountId}"]`);
        if (!row) return;

        // Update row content
        const newRow = this.createTableRow(account);
        row.innerHTML = newRow.innerHTML;
        
        // Highlight updated row
        row.classList.add('table-warning');
        setTimeout(() => row.classList.remove('table-warning'), 3000);
    }

    removeAccountFromTable(accountId) {
        const row = document.querySelector(`tr[data-account-id="${accountId}"]`);
        if (row) {
            row.classList.add('table-danger');
            setTimeout(() => row.remove(), 1000);
        }
    }

    updateAccountDetails(account) {
        // Update details page if currently viewing this account
        if (window.location.pathname.includes(account.accountId)) {
            const nameElement = document.querySelector('.account-name');
            const emailElement = document.querySelector('.account-email');
            const roleElement = document.querySelector('.account-role');
            
            if (nameElement) nameElement.textContent = account.accountName;
            if (emailElement) emailElement.textContent = account.accountEmail;
            if (roleElement) roleElement.textContent = account.accountRoleName;
        }
    }

    updateCurrentUserProfile(account) {
        // Update current user's profile information displayed on page
        const profileElements = document.querySelectorAll('.current-user-name');
        profileElements.forEach(element => {
            element.textContent = account.accountName;
        });
    }

    updateUserOnlineStatus(accountId, isOnline) {
        const statusElement = document.querySelector(`tr[data-account-id="${accountId}"] .online-status`);
        if (statusElement) {
            statusElement.innerHTML = isOnline ? 
                '<span class="badge bg-success">Online</span>' : 
                '<span class="badge bg-secondary">Offline</span>';
        }
    }

    createTableRow(account) {
        const row = document.createElement('tr');
        row.setAttribute('data-account-id', account.accountId);
        
        const roleNames = {
            1: 'Member',
            2: 'Staff',
            3: 'Admin'
        };
        
        const roleName = roleNames[account.accountRole] || 'Unknown';
        const roleBadgeClass = account.accountRole === 3 ? 'bg-danger' : 
                              account.accountRole === 2 ? 'bg-warning' : 'bg-info';
        
        row.innerHTML = `
            <td>${account.accountName}</td>
            <td>${account.accountEmail}</td>
            <td><span class="badge ${roleBadgeClass}">${roleName}</span></td>
            <td>${account.newsArticleCount || 0}</td>
            <td class="online-status"><span class="badge bg-secondary">Offline</span></td>
            <td>
                <a href="/Account/Edit?id=${account.accountId}" class="btn btn-sm btn-warning">Edit</a>
                <button class="btn btn-sm btn-danger" onclick="deleteAccount(${account.accountId})">Delete</button>
            </td>
        `;
        
        return row;
    }

    updateCounters() {
        // Update any counters on the page
        const counterElements = document.querySelectorAll('.account-count');
        counterElements.forEach(element => {
            if (typeof updateAccountCount === 'function') {
                updateAccountCount();
            }
        });
    }

    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(notification);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }
}

// Global instance
window.accountRealTime = new AccountRealTime();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.accountRealTime.initialize();
});
