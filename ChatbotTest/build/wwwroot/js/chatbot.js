let connection = false; // Set to true when you want to enable API requests

function toggleChat() {
    const chatWindow = document.getElementById('chatbot-window');
    chatWindow.style.display = chatWindow.style.display === 'none' ? 'flex' : 'none';
}

function checkEnter(e) {
    if (e.key === 'Enter') sendMessage();
}

function sendMessage() {
    const input = document.getElementById('chatbot-text');
    const message = input.value.trim();
    if (!message) return;

    appendMessage('user', message);
    input.value = '';

    if (connection) {
        fetch('/api/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message })
        })
            .then(res => res.json())
            .then(data => {
                appendMessage('bot', data.response || '[No response]');
            })
            .catch(err => {
                console.error(err);
                appendMessage('bot', '[Error reaching the assistant]');
            });
    } else {
        // Offline fallback
        setTimeout(() => {
            appendMessage('bot', 'This is a placeholder response.');
        }, 500);
    }
}

function appendMessage(sender, text) {
    const container = document.getElementById('chatbot-messages');

    const msg = document.createElement('div');
    msg.className = `chatmsg ${sender}`;
    msg.textContent = text;

    container.appendChild(msg);

    // Force reflow (browser recalculates layout)
    void msg.offsetHeight;

    // Now scroll to bottom
    container.scrollTop = container.scrollHeight;
}

