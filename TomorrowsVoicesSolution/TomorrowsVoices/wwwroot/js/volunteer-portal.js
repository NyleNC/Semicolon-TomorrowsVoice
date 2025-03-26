// Volunteer Portal JavaScript

// Initialize event filters
function initEventFilters() {
    $('#eventFilter').change(function() {
        const filter = $(this).val();
        const today = new Date().toISOString().split('T')[0].replace(/-/g, '');
        const oneWeekFromNow = new Date();
        oneWeekFromNow.setDate(oneWeekFromNow.getDate() + 7);
        const weekEnd = oneWeekFromNow.toISOString().split('T')[0].replace(/-/g, '');
        const oneMonthFromNow = new Date();
        oneMonthFromNow.setMonth(oneMonthFromNow.getMonth() + 1);
        const monthEnd = oneMonthFromNow.toISOString().split('T')[0].replace(/-/g, '');
        
        $('.event-card-container').each(function() {
            const eventDate = $(this).data('event-date');
            
            if (filter === 'all') {
                $(this).show();
            } else if (filter === 'today' && eventDate === today) {
                $(this).show();
            } else if (filter === 'week' && eventDate >= today && eventDate <= weekEnd) {
                $(this).show();
            } else if (filter === 'month' && eventDate >= today && eventDate <= monthEnd) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
}

// Initialize countdown timers
function initCountdownTimers() {
    // Update all counters every minute
    setInterval(updateCountdowns, 60000);
    
    // Initial update
    updateCountdowns();
}

function updateCountdowns() {
    $('.countdown').each(function() {
        const countdownElement = $(this);
        const shiftRow = countdownElement.closest('tr');
        const timeString = countdownElement.text();
        
        // If already says "Now", skip
        if (timeString === "Now") return;
        
        // Extract hours, days, minutes
        let value, unit;
        if (timeString.includes("day")) {
            value = parseInt(timeString);
            unit = "days";
        } else if (timeString.includes("hour")) {
            value = parseInt(timeString);
            unit = "hours";
        } else if (timeString.includes("min")) {
            value = parseInt(timeString);
            unit = "minutes";
        } else {
            return; // Unknown format
        }
        
        // Decrease the value
        if (unit === "minutes") {
            value -= 1;
            if (value <= 0) {
                countdownElement.text("Now");
                
                // Check if there's a check-in button to enable
                const checkInBtn = shiftRow.find('.btn-success');
                if (checkInBtn.length > 0 && checkInBtn.is(':disabled')) {
                    checkInBtn.prop('disabled', false);
                    
                    // Add a visual indicator that the button is now enabled
                    checkInBtn.addClass('pulse-animation');
                    
                    // Show a notification
                    showNotification("You can now check in for your shift!");
                }
                return;
            }
            countdownElement.text(value + " min");
        } else if (unit === "hours") {
            value -= 1;
            if (value <= 0) {
                countdownElement.text("Now");
                return;
            }
            
            const plural = value > 1 ? "s" : "";
            countdownElement.text(value + " hour" + plural);
        } else if (unit === "days") {
            // No need to update days in real-time
        }
    });
}

// Start the check-in countdown
function startCheckInCountdown(seconds) {
    const countdownElement = document.getElementById('countdownTimer');
    const checkInButton = document.getElementById('checkInButton');
    
    if (!countdownElement || !checkInButton) return;
    
    // If button should already be enabled
    if (seconds <= 0) {
        checkInButton.disabled = false;
        countdownElement.textContent = "Now";
        return;
    }
    
    // Otherwise start countdown
    checkInButton.disabled = true;
    
    const countdownInterval = setInterval(() => {
        seconds -= 1;
        
        if (seconds <= 0) {
            clearInterval(countdownInterval);
            countdownElement.textContent = "Now";
            checkInButton.disabled = false;
            checkInButton.classList.add('pulse-animation');
            showNotification("You can now check in for your shift!");
        } else {
            const minutes = Math.floor(seconds / 60);
            const remainingSeconds = seconds % 60;
            countdownElement.textContent = `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
        }
    }, 1000);
}

// Show a browser notification
function showNotification(message) {
    // Check if the browser supports notifications
    if (!("Notification" in window)) {
        alert(message);
        return;
    }
    
    // Check if permission is already granted
    if (Notification.permission === "granted") {
        new Notification("Tomorrow's Voices", {
            body: message,
            icon: "/favicon.ico"
        });
    }
    // Otherwise, request permission
    else if (Notification.permission !== "denied") {
        Notification.requestPermission().then(function (permission) {
            if (permission === "granted") {
                new Notification("Tomorrow's Voices", {
                    body: message,
                    icon: "/favicon.ico"
                });
            }
        });
    }
}

// DOM Ready
$(document).ready(function() {
    // Request notification permission on page load
    if ("Notification" in window && Notification.permission !== "denied") {
        Notification.requestPermission();
    }
    
    // Add pulse animation
    $("<style>")
        .prop("type", "text/css")
        .html(`
            @keyframes pulse {
                0% { transform: scale(1); }
                50% { transform: scale(1.1); }
                100% { transform: scale(1); }
            }
            .pulse-animation {
                animation: pulse 1s infinite;
            }
        `)
        .appendTo("head");
});
