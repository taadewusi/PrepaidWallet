// Modal helpers
function openModal(id) {
    const el = document.getElementById(id);
    if (el) {
        el.classList.add('open');
        // Focus first input
        setTimeout(() => el.querySelector('input')?.focus(), 50);
    }
}

function closeModal(id) {
    const el = document.getElementById(id);
    if (el) el.classList.remove('open');
}

// Close modal on overlay click
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal-overlay')) {
        e.target.classList.remove('open');
    }
});

// Close modal on Escape
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-overlay.open')
            .forEach(el => el.classList.remove('open'));
    }
});

// Auto-dismiss alerts after 5s
document.querySelectorAll('.alert-toast').forEach(el => {
    setTimeout(() => el.remove(), 5000);
});
