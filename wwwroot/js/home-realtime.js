// Home Page Real-time functionality
class HomeRealTime {
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

        this.connection = window.signalRClient.getConnection('category');
        this.setupEventHandlers();
        this.isInitialized = true;
        console.log("Home real-time initialized");
    }

    setupEventHandlers() {
        // Category List Updated - refresh category stats
        this.connection.on("CategoryListUpdated", () => {
            this.refreshCategoryStats();
        });

        // Category Created
        this.connection.on("CategoryCreated", (category) => {
            this.handleCategoryCreated(category);
        });

        // Category Updated
        this.connection.on("CategoryUpdated", (category) => {
            this.handleCategoryUpdated(category);
        });

        // Category Deleted
        this.connection.on("CategoryDeleted", (categoryId) => {
            this.handleCategoryDeleted(categoryId);
        });
    }

    handleCategoryCreated(category) {
        console.log("New category created:", category);
        this.addCategoryToList(category);
        this.showNotification(`New category created: ${category.categoryName}`, 'success');
    }

    handleCategoryUpdated(category) {
        console.log("Category updated:", category);
        this.updateCategoryInList(category);
        this.showNotification(`Category updated: ${category.categoryName}`, 'info');
    }

    handleCategoryDeleted(categoryId) {
        console.log("Category deleted:", categoryId);
        this.removeCategoryFromList(categoryId);
        this.showNotification('Category deleted', 'warning');
    }

    addCategoryToList(category) {
        if (!category.isActive) return; // Only show active categories

        const categoryList = document.querySelector('.card-body .list-group');
        const noCategoriesMessage = document.querySelector('.card-body .text-muted');
        
        if (noCategoriesMessage && noCategoriesMessage.textContent.includes('No categories found')) {
            noCategoriesMessage.remove();
        }

        if (!categoryList) {
            // Create the list if it doesn't exist
            const cardBody = document.querySelector('.card-body');
            if (cardBody) {
                const newList = document.createElement('div');
                newList.className = 'list-group';
                cardBody.appendChild(newList);
            }
        }

        const categoryItem = this.createCategoryListItem(category);
        const existingList = document.querySelector('.card-body .list-group');
        if (existingList) {
            existingList.appendChild(categoryItem);
        }
    }

    updateCategoryInList(category) {
        const existingItem = document.querySelector(`[data-category-id="${category.categoryId}"]`);
        if (existingItem) {
            if (category.isActive) {
                // Update existing item
                const newItem = this.createCategoryListItem(category);
                existingItem.replaceWith(newItem);
            } else {
                // Remove if inactive
                existingItem.remove();
                this.checkEmptyList();
            }
        } else if (category.isActive) {
            // Add if it's now active
            this.addCategoryToList(category);
        }
    }

    removeCategoryFromList(categoryId) {
        const categoryItem = document.querySelector(`[data-category-id="${categoryId}"]`);
        if (categoryItem) {
            categoryItem.remove();
            this.checkEmptyList();
        }
    }

    createCategoryListItem(category) {
        const item = document.createElement('div');
        item.className = 'list-group-item';
        item.setAttribute('data-category-id', category.categoryId);
        
        item.innerHTML = `
            <div class="d-flex w-100 justify-content-between">
                <h6 class="mb-1">${category.categoryName}</h6>
                <span class="badge bg-primary rounded-pill">${category.newsArticleCount || 0}</span>
            </div>
            <small class="text-muted">${category.categoryDesciption}</small>
        `;
        
        return item;
    }

    checkEmptyList() {
        const categoryList = document.querySelector('.card-body .list-group');
        if (categoryList && categoryList.children.length === 0) {
            const cardBody = document.querySelector('.card-body');
            if (cardBody) {
                const noDataMessage = document.createElement('p');
                noDataMessage.className = 'text-muted';
                noDataMessage.textContent = 'No categories found.';
                cardBody.appendChild(noDataMessage);
                categoryList.remove();
            }
        }
    }

    async refreshCategoryStats() {
        try {
            // Fetch updated category stats from server
            const response = await fetch('/Home/GetCategoryStats', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (response.ok) {
                const categoryStats = await response.json();
                this.updateCategoryList(categoryStats);
            }
        } catch (error) {
            console.error('Error refreshing category stats:', error);
        }
    }

    updateCategoryList(categoryStats) {
        const cardBody = document.querySelector('.card-body');
        if (!cardBody) return;

        // Clear existing content
        cardBody.innerHTML = '';

        if (categoryStats && categoryStats.length > 0) {
            const listGroup = document.createElement('div');
            listGroup.className = 'list-group';

            categoryStats.forEach(stat => {
                const item = this.createCategoryListItem({
                    categoryId: stat.category.categoryId,
                    categoryName: stat.category.categoryName,
                    categoryDesciption: stat.category.categoryDesciption,
                    newsArticleCount: stat.articleCount,
                    isActive: stat.category.isActive
                });
                listGroup.appendChild(item);
            });

            cardBody.appendChild(listGroup);
        } else {
            const noDataMessage = document.createElement('p');
            noDataMessage.className = 'text-muted';
            noDataMessage.textContent = 'No categories found.';
            cardBody.appendChild(noDataMessage);
        }
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

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    if (typeof window.signalRClient !== 'undefined') {
        window.homeRealTime = new HomeRealTime();
        
        // Wait a bit for SignalR to initialize
        setTimeout(() => {
            window.homeRealTime.initialize();
        }, 2000);
    }
});
