// Category Real-time functionality
class CategoryRealTime {
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
        console.log("Category real-time initialized");
    }

    setupEventHandlers() {
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

        // Category Status Changed
        this.connection.on("CategoryStatusChanged", (categoryId, isActive) => {
            this.handleCategoryStatusChanged(categoryId, isActive);
        });

        // Category List Updated (for navigation updates)
        this.connection.on("CategoryListUpdated", () => {
            this.handleCategoryListUpdated();
        });
    }

    handleCategoryCreated(category) {
        console.log("New category created:", category);
        
        // Show notification
        this.showNotification(`New category created: ${category.categoryName}`, 'success');
        
        // Add to table if on index page
        this.addCategoryToTable(category);
        
        // Update navigation
        this.updateNavigation();
        
        // Update counters
        this.updateCounters();
    }

    handleCategoryUpdated(category) {
        console.log("Category updated:", category);
        
        // Show notification
        this.showNotification(`Category updated: ${category.categoryName}`, 'info');
        
        // Update table row if exists
        this.updateCategoryInTable(category);
        
        // Update navigation
        this.updateNavigation();
        
        // Update details if on details page
        this.updateCategoryDetails(category);
    }

    handleCategoryDeleted(categoryId) {
        console.log("Category deleted:", categoryId);
        
        // Show notification
        this.showNotification('Category has been deleted', 'warning');
        
        // Remove from table
        this.removeCategoryFromTable(categoryId);
        
        // Update navigation
        this.updateNavigation();
        
        // Redirect if on deleted category's page
        if (window.location.pathname.includes(categoryId)) {
            window.location.href = '/Category';
        }
        
        // Update counters
        this.updateCounters();
    }

    handleCategoryStatusChanged(categoryId, isActive) {
        console.log("Category status changed:", categoryId, isActive);
        
        // Update status in table
        this.updateCategoryStatus(categoryId, isActive);
        
        // Update navigation
        this.updateNavigation();
        
        // Show notification
        const statusText = isActive ? 'Activated' : 'Deactivated';
        this.showNotification(`Category ${statusText}`, 'info');
    }

    handleCategoryListUpdated() {
        console.log("Category list updated");
        this.updateNavigation();
    }

    addCategoryToTable(category) {
        const tableBody = document.querySelector('#categoriesTable tbody');
        if (!tableBody) return;

        const row = this.createTableRow(category);
        tableBody.insertBefore(row, tableBody.firstChild);
        
        // Highlight new row
        row.classList.add('table-success');
        setTimeout(() => row.classList.remove('table-success'), 3000);
    }

    updateCategoryInTable(category) {
        const row = document.querySelector(`tr[data-category-id="${category.categoryId}"]`);
        if (!row) return;

        // Update row content
        const newRow = this.createTableRow(category);
        row.innerHTML = newRow.innerHTML;
        
        // Highlight updated row
        row.classList.add('table-warning');
        setTimeout(() => row.classList.remove('table-warning'), 3000);
    }

    removeCategoryFromTable(categoryId) {
        const row = document.querySelector(`tr[data-category-id="${categoryId}"]`);
        if (row) {
            row.classList.add('table-danger');
            setTimeout(() => row.remove(), 1000);
        }
    }

    updateCategoryStatus(categoryId, isActive) {
        const statusCell = document.querySelector(`tr[data-category-id="${categoryId}"] .status-cell`);
        if (statusCell) {
            statusCell.innerHTML = isActive ? 
                '<span class="badge bg-success">Active</span>' : 
                '<span class="badge bg-secondary">Inactive</span>';
        }
    }

    updateCategoryDetails(category) {
        // Update details page if currently viewing this category
        if (window.location.pathname.includes(category.categoryId)) {
            const nameElement = document.querySelector('.category-name');
            const descriptionElement = document.querySelector('.category-description');
            
            if (nameElement) nameElement.textContent = category.categoryName;
            if (descriptionElement) descriptionElement.textContent = category.categoryDesciption;
        }
    }

    createTableRow(category) {
        const row = document.createElement('tr');
        row.setAttribute('data-category-id', category.categoryId);
        
        const statusBadge = category.isActive ? 
            '<span class="badge bg-success">Active</span>' : 
            '<span class="badge bg-secondary">Inactive</span>';
        
        row.innerHTML = `
            <td>${category.categoryName}</td>
            <td>${category.categoryDesciption}</td>
            <td>${category.parentCategoryName || 'None'}</td>
            <td class="status-cell">${statusBadge}</td>
            <td>${category.newsArticleCount || 0}</td>
            <td>
                <a href="/Category/Edit?id=${category.categoryId}" class="btn btn-sm btn-warning">Edit</a>
                <button class="btn btn-sm btn-danger" onclick="deleteCategory(${category.categoryId})">Delete</button>
            </td>
        `;
        
        return row;
    }

    updateNavigation() {
        // Update category navigation/dropdown if exists
        const categoryNav = document.querySelector('.category-navigation');
        if (categoryNav) {
            // This would need to fetch updated category list
            // For now, we'll just trigger a refresh
            if (typeof refreshCategoryNavigation === 'function') {
                refreshCategoryNavigation();
            }
        }
    }

    updateCounters() {
        // Update any counters on the page
        const counterElements = document.querySelectorAll('.category-count');
        counterElements.forEach(element => {
            if (typeof updateCategoryCount === 'function') {
                updateCategoryCount();
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
window.categoryRealTime = new CategoryRealTime();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.categoryRealTime.initialize();
});
