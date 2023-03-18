// Check local storage for user preference and switch icon state
const isDarkMode = localStorage.getItem('darkMode') === 'true';
const isIconMoon = localStorage.getItem('iconState') === 'moon';
document.body.classList.toggle('dark-mode', isDarkMode);
document.getElementById('dark-mode-toggle').checked = isDarkMode;
const icon = document.getElementById('dark-mode-icon');
if (isIconMoon) {
    icon.classList.remove('bi-sun');
    icon.classList.add('bi-moon');
}

// Handle button click to toggle dark mode and switch icon
document.getElementById('dark-mode-toggle').addEventListener('click', function () {
    document.body.classList.toggle('dark-mode');
    // Update local storage with user preference and switch icon state
    localStorage.setItem('darkMode', document.body.classList.contains('dark-mode'));
    if (document.body.classList.contains('dark-mode')) {
        icon.classList.remove('bi-sun');
        icon.classList.add('bi-moon');
        localStorage.setItem('iconState', 'moon');
    } else {
        icon.classList.remove('bi-moon');
        icon.classList.add('bi-sun');
        localStorage.setItem('iconState', 'sun');
    }
    document.getElementById('dark-mode-toggle').checked = document.body.classList.contains('dark-mode');
});
