// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Fix for bootstrap-select initialization
$(document).ready(function() {
    // Check if Bootstrap is available before initializing bootstrap-select
    if (typeof $.fn.modal !== 'undefined') {
        // Initialize bootstrap-select if it exists
        if (typeof $.fn.selectpicker !== 'undefined') {
            $('.selectpicker').selectpicker();
        }
    } else {
        console.warn('Bootstrap is not loaded before bootstrap-select');
    }

    // Fix for accessibility issues with modals
    $('.modal').each(function() {
        // Use inert attribute instead of aria-hidden
        $(this).attr('inert', '');
    });

    // Fix for aria-hidden issues
    $('.modal').on('show.bs.modal', function() {
        // Remove inert attribute when modal is shown
        $(this).removeAttr('inert');
        // Remove aria-hidden from modal when shown
        $(this).removeAttr('aria-hidden');
    }).on('hidden.bs.modal', function() {
        // Add inert attribute when modal is hidden
        $(this).attr('inert', '');
    });

    // Fix invalid form control focusability
    $('input[required], select[required], textarea[required]').each(function() {
        // Ensure required fields have an initial value to avoid validation errors
        if ($(this).attr('type') === 'time' && !$(this).val()) {
            $(this).val('09:00');
        }
        
        // Ensure all required fields are focusable
        if (!$(this).attr('tabindex')) {
            $(this).attr('tabindex', '0');
        }
    });

    // Add default values to time inputs to avoid the "invalid form control" error
    $('input[type="time"]').each(function() {
        if (!$(this).val()) {
            $(this).val('09:00');
        }
    });
    
    // Fix focus management in modals
    $('.modal').on('shown.bs.modal', function() {
        // Find the first focusable element in the modal and focus it
        const focusableElements = $(this).find('a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])').filter(':visible');
        if (focusableElements.length > 0) {
            setTimeout(() => {
                focusableElements[0].focus();
            }, 100);
        }
    });
    
    // Setup keyboard trap for modals to keep focus inside
    $('.modal').on('keydown', function(e) {
        // Only process tab key
        if (e.key === 'Tab' || e.keyCode === 9) {
            const focusableElements = $(this).find('a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])').filter(':visible');
            const firstElement = focusableElements[0];
            const lastElement = focusableElements[focusableElements.length - 1];
            
            // Handle cycling through elements with Tab
            if (!e.shiftKey && document.activeElement === lastElement) {
                firstElement.focus();
                e.preventDefault();
            } else if (e.shiftKey && document.activeElement === firstElement) {
                lastElement.focus();
                e.preventDefault();
            }
        }
    });
});
