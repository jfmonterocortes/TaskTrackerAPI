// Global utility functions for the entire application

// Show notification message (could be enhanced with toast notifications)
function showNotification(message, type = 'info') {
    // Simple implementation - could use a proper toast library
    console.log(`${type.toUpperCase()}: ${message}`);

    // For now, using alert as fallback
    if (type === 'error') {
        alert('Error: ' + message);
    } else {
        alert(message);
    }
}

// Format date for display
function formatDate(dateString) {
    if (!dateString) return 'Not set';

    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

// Debounce function for search inputs
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Export for use in other modules (if using modules)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { showNotification, formatDate, debounce };
}