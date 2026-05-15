const API_BASE = '/api';
let authToken = localStorage.getItem('wallet_token') || null;
let currentStudentId = localStorage.getItem('student_id') || null;

// DOM Elements
const loginScreen = document.getElementById('login-screen');
const dashboard = document.getElementById('dashboard');
const historyList = document.getElementById('history-list');
const balanceEl = document.getElementById('balance');
const studentNameEl = document.getElementById('student-name');
const actionType = document.getElementById('action-type');
const transferField = document.getElementById('transfer-field');
const serviceField = document.getElementById('service-field');

// Initialize
if (authToken) {
    showDashboard();
}

// Auth Toggle
let isLoginMode = true;
const authBtn = document.getElementById('auth-btn');
const authTitle = document.getElementById('auth-title');
const authSubtitle = document.getElementById('auth-subtitle');
const signupFields = document.getElementById('signup-fields');
const toggleAuth = document.getElementById('toggle-auth');
const toggleText = document.getElementById('toggle-text');
const authError = document.getElementById('auth-error');

toggleAuth.addEventListener('click', (e) => {
    e.preventDefault();
    isLoginMode = !isLoginMode;
    
    authTitle.textContent = isLoginMode ? 'ATM Management System' : 'Create Account';
    authSubtitle.textContent = isLoginMode ? 'Welcome back! Please login.' : 'Join us today! It only takes a minute.';
    authBtn.textContent = isLoginMode ? 'Login Securely' : 'Sign Up Now';
    toggleText.textContent = isLoginMode ? "Don't have an account?" : 'Already have an account?';
    toggleAuth.textContent = isLoginMode ? 'Create Account' : 'Login';
    signupFields.classList.toggle('hidden', isLoginMode);
    authError.classList.add('hidden');
});

// Auth Execution (Login or Register)
authBtn.addEventListener('click', async () => {
    const studentId = document.getElementById('stu-id').value;
    const pin = document.getElementById('stu-pin').value;
    const fullName = document.getElementById('stu-name-input').value;
    
    authError.classList.add('hidden');

    const endpoint = isLoginMode ? '/auth/login' : '/auth/register';
    const body = isLoginMode ? { studentId, pin } : { studentId, pin, fullName };

    try {
        console.time('auth-request');
        const res = await fetch(`${API_BASE}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });
        console.timeEnd('auth-request');

        const data = await res.json();
        if (data.status === 'success') {
            if (isLoginMode) {
                authToken = data.data;
                currentStudentId = studentId;
                localStorage.setItem('wallet_token', authToken);
                localStorage.setItem('student_id', currentStudentId);
                showDashboard();
            } else {
                alert(data.message);
                // Clear sign up fields
                document.getElementById('stu-id').value = '';
                document.getElementById('stu-pin').value = '';
                document.getElementById('stu-name-input').value = '';
                toggleAuth.click(); // Switch back to login
            }
        } else {
            authError.textContent = data.message;
            authError.classList.remove('hidden');
        }
    } catch (err) {
        console.error("Auth Error:", err);
        authError.textContent = "Server error. Is the API running?";
        authError.classList.remove('hidden');
    }
});

// Logout
document.getElementById('logout-btn').addEventListener('click', () => {
    localStorage.clear();
    location.reload();
});

// Action UI Toggle
actionType.addEventListener('change', (e) => {
    transferField.classList.add('hidden');
    serviceField.classList.add('hidden');
    
    if (e.target.value === 'transfer') transferField.classList.remove('hidden');
    if (e.target.value === 'pay') serviceField.classList.remove('hidden');
});

// Execute Transaction
document.getElementById('execute-btn').addEventListener('click', async () => {
    const type = actionType.value;
    const amount = parseFloat(document.getElementById('action-amount').value);
    
    let endpoint = '';
    let body = { amount };

    if (type === 'deposit') endpoint = '/transaction/deposit';
    if (type === 'pay') {
        endpoint = '/transaction/pay';
        body.serviceType = parseInt(document.getElementById('service-type').value);
    }
    if (type === 'transfer') {
        endpoint = '/transaction/transfer';
        body.receiverStudentId = document.getElementById('receiver-id').value;
    }

    try {
        const res = await fetch(`${API_BASE}${endpoint}`, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(body)
        });

        const data = await res.json();
        alert(data.message);
        if (data.status === 'success') {
            loadWalletData();
        }
    } catch (err) {
        alert("Transaction failed");
    }
});

async function showDashboard() {
    loginScreen.classList.add('hidden');
    dashboard.classList.remove('hidden');
    loadWalletData();
}

async function loadWalletData() {
    // Get Profile
    const profileRes = await fetch(`${API_BASE}/wallet/profile`, {
        headers: { 'Authorization': `Bearer ${authToken}` }
    });
    const profileData = await profileRes.json();
    
    if (profileData.status === 'success') {
        studentNameEl.textContent = profileData.data.fullName;
        balanceEl.textContent = profileData.data.wallet.balance.toLocaleString();
        
        // Ensure student ID is set for CSV download
        currentStudentId = profileData.data.studentId;
        localStorage.setItem('student_id', currentStudentId);
    }

    // Get History
    const historyRes = await fetch(`${API_BASE}/report/history/${currentStudentId}`, {
        headers: { 'Authorization': `Bearer ${authToken}` }
    });
    const historyData = await historyRes.json();
    
    if (historyData.status === 'success') {
        renderHistory(historyData.data);
    }
}

function renderHistory(transactions) {
    historyList.innerHTML = '';
    if (transactions.length === 0) {
        historyList.innerHTML = '<p style="text-align:center; color:var(--text-muted);">No transactions yet.</p>';
        return;
    }

    transactions.forEach(tx => {
        const isPositive = tx.type === 'DEPOSIT' || tx.type === 'TRANSFER_IN';
        const item = document.createElement('div');
        item.className = 'tx-item';
        item.innerHTML = `
            <div>
                <div class="tx-type">${tx.type}</div>
                <div style="color:var(--text-muted); font-size:0.7rem;">${new Date(tx.timestamp).toLocaleString()}</div>
            </div>
            <div class="tx-amount ${isPositive ? 'positive' : 'negative'}">
                ${isPositive ? '+' : '-'}${tx.amount.toLocaleString()}
            </div>
        `;
        historyList.appendChild(item);
    });
}

// Download CSV
document.getElementById('download-csv-btn').addEventListener('click', () => {
    if (!authToken) {
        alert("Please log in first.");
        return;
    }
    // Use direct link for more reliable browser download handling
    const downloadUrl = `${API_BASE}/report/export-csv?token=${encodeURIComponent(authToken)}`;
    window.location.href = downloadUrl;
});
