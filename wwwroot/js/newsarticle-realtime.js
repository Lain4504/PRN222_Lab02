// News Article Real-time functionality
class NewsArticleRealTime {
    constructor() {
        this.connection = null;
        this.currentCategoryId = null;
        this.isInitialized = false;
    }

    async initialize() {
        if (!window.signalRClient || !window.signalRClient.isConnectionActive()) {
            console.log("Waiting for SignalR connection...");
            setTimeout(() => this.initialize(), 1000);
            return;
        }

        this.connection = window.signalRClient.getConnection('newsArticle');
        this.setupEventHandlers();
        this.isInitialized = true;
        console.log("News Article real-time initialized");
    }

    setupEventHandlers() {
        // Article Created
        this.connection.on("ArticleCreated", (article) => {
            this.handleArticleCreated(article);
        });

        // Article Updated
        this.connection.on("ArticleUpdated", (article) => {
            this.handleArticleUpdated(article);
        });

        // Article Deleted
        this.connection.on("ArticleDeleted", (articleId) => {
            this.handleArticleDeleted(articleId);
        });

        // Article Status Changed
        this.connection.on("ArticleStatusChanged", (articleId, status) => {
            this.handleArticleStatusChanged(articleId, status);
        });
    }

    handleArticleCreated(article) {
        console.log("New article created:", article);
        
        // Show notification
        this.showNotification(`New article created: ${article.newsTitle}`, 'success');
        
        // Add to table if on index page
        this.addArticleToTable(article);
        
        // Update counters
        this.updateCounters();
    }

    handleArticleUpdated(article) {
        console.log("Article updated:", article);
        
        // Show notification
        this.showNotification(`Article updated: ${article.newsTitle}`, 'info');
        
        // Update table row if exists
        this.updateArticleInTable(article);
        
        // Update details if on details page
        this.updateArticleDetails(article);
    }

    handleArticleDeleted(articleId) {
        console.log("Article deleted:", articleId);
        
        // Show notification
        this.showNotification('Article has been deleted', 'warning');
        
        // Remove from table
        this.removeArticleFromTable(articleId);
        
        // Redirect if on deleted article's page
        if (window.location.pathname.includes(articleId)) {
            window.location.href = '/NewsArticle';
        }
        
        // Update counters
        this.updateCounters();
    }

    handleArticleStatusChanged(articleId, status) {
        console.log("Article status changed:", articleId, status);
        
        // Update status in table
        this.updateArticleStatus(articleId, status);
        
        // Show notification
        const statusText = status ? 'Published' : 'Unpublished';
        this.showNotification(`Article ${statusText}`, 'info');
    }

    addArticleToTable(article) {
        const tableBody = document.querySelector('#articlesTable tbody');
        if (!tableBody) return;

        const row = this.createTableRow(article);
        tableBody.insertBefore(row, tableBody.firstChild);
        
        // Highlight new row
        row.classList.add('table-success');
        setTimeout(() => row.classList.remove('table-success'), 3000);
    }

    updateArticleInTable(article) {
        const row = document.querySelector(`tr[data-article-id="${article.newsArticleId}"]`);
        if (!row) return;

        // Update row content
        const newRow = this.createTableRow(article);
        row.innerHTML = newRow.innerHTML;
        
        // Highlight updated row
        row.classList.add('table-warning');
        setTimeout(() => row.classList.remove('table-warning'), 3000);
    }

    removeArticleFromTable(articleId) {
        const row = document.querySelector(`tr[data-article-id="${articleId}"]`);
        if (row) {
            row.classList.add('table-danger');
            setTimeout(() => row.remove(), 1000);
        }
    }

    updateArticleStatus(articleId, status) {
        const statusCell = document.querySelector(`tr[data-article-id="${articleId}"] .status-cell`);
        if (statusCell) {
            statusCell.innerHTML = status ? 
                '<span class="badge bg-success">Published</span>' : 
                '<span class="badge bg-secondary">Draft</span>';
        }
    }

    updateArticleDetails(article) {
        // Update details page if currently viewing this article
        if (window.location.pathname.includes(article.newsArticleId)) {
            const titleElement = document.querySelector('.article-title');
            const headlineElement = document.querySelector('.article-headline');
            const contentElement = document.querySelector('.article-content');
            
            if (titleElement) titleElement.textContent = article.newsTitle;
            if (headlineElement) headlineElement.textContent = article.headline;
            if (contentElement) contentElement.innerHTML = article.newsContent;
        }
    }

    createTableRow(article) {
        const row = document.createElement('tr');
        row.setAttribute('data-article-id', article.newsArticleId);
        
        const statusBadge = article.newsStatus ? 
            '<span class="badge bg-success">Published</span>' : 
            '<span class="badge bg-secondary">Draft</span>';
            
        const formattedDate = new Date(article.createdDate).toLocaleDateString();
        
        row.innerHTML = `
            <td>${article.newsTitle || 'Untitled'}</td>
            <td>${article.headline}</td>
            <td>${article.categoryName || 'No Category'}</td>
            <td>${article.createdByName || 'Unknown'}</td>
            <td>${formattedDate}</td>
            <td class="status-cell">${statusBadge}</td>
            <td>
                <a href="/NewsArticle/Details?id=${article.newsArticleId}" class="btn btn-sm btn-info">Details</a>
                <a href="/NewsArticle/Edit?id=${article.newsArticleId}" class="btn btn-sm btn-warning">Edit</a>
                <button class="btn btn-sm btn-danger" onclick="deleteArticle('${article.newsArticleId}')">Delete</button>
            </td>
        `;
        
        return row;
    }

    updateCounters() {
        // Update any counters on the page
        const counterElements = document.querySelectorAll('.article-count');
        counterElements.forEach(element => {
            // This would need to be implemented based on your specific counter logic
            // For now, we'll just trigger a refresh of the counter
            if (typeof updateArticleCount === 'function') {
                updateArticleCount();
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

    // Join specific category group
    async joinCategoryGroup(categoryId) {
        if (this.connection && categoryId) {
            await this.connection.invoke("JoinCategoryGroup", categoryId.toString());
            this.currentCategoryId = categoryId;
        }
    }

    // Leave current category group
    async leaveCategoryGroup() {
        if (this.connection && this.currentCategoryId) {
            await this.connection.invoke("LeaveCategoryGroup", this.currentCategoryId.toString());
            this.currentCategoryId = null;
        }
    }
}

// Global instance
window.newsArticleRealTime = new NewsArticleRealTime();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.newsArticleRealTime.initialize();
});
