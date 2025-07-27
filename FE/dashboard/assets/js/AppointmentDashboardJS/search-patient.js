// search-patient.js
// Xử lý chức năng tìm kiếm bệnh nhân
(function() {
    const API_BASE_URL = 'https://localhost:7097';

    // Search patients
    window.searchPatients = function() {
        const searchTerm = document.getElementById('searchPatientInput').value.trim();
        const searchResults = document.getElementById('searchResults');
        
        if (!searchTerm) {
            searchResults.innerHTML = '';
            return;
        }

        // Show loading
        searchResults.innerHTML = '<div class="text-center"><i class="fas fa-spinner fa-spin"></i> Đang tìm kiếm...</div>';

        fetch(`${API_BASE_URL}/api/appointment/patient/search?term=${encodeURIComponent(searchTerm)}`)
            .then(res => res.json())
            .then(data => {
                if (data && data.length > 0) {
                    let html = '<div class="list-group">';
                    data.forEach(patient => {
                        html += `
                            <div class="list-group-item list-group-item-action" onclick="selectPatient(${patient.id}, '${patient.name}', '${patient.email || ''}', '${patient.phone || ''}')">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">${patient.name}</h6>
                                    <small>ID: ${patient.id}</small>
                                </div>
                                <p class="mb-1">Email: ${patient.email || 'N/A'}</p>
                                <small>Phone: ${patient.phone || 'N/A'}</small>
                            </div>
                        `;
                    });
                    html += '</div>';
                    searchResults.innerHTML = html;
                } else {
                    searchResults.innerHTML = '<div class="text-center text-muted">Không tìm thấy bệnh nhân</div>';
                }
            })
            .catch(err => {
                console.error('Error searching patients:', err);
                searchResults.innerHTML = '<div class="text-center text-danger">Lỗi khi tìm kiếm</div>';
            });
    };

    // Select patient from search results
    window.selectPatient = function(patientId, patientName, patientEmail, patientPhone) {
        // Fill the form fields
        const nameInput = document.getElementById('patient_name');
        const emailInput = document.getElementById('patient_email');
        const phoneInput = document.getElementById('patient_phone');
        
        if (nameInput) nameInput.value = patientName;
        if (emailInput) emailInput.value = patientEmail;
        if (phoneInput) phoneInput.value = patientPhone;
        
        // Clear search results
        document.getElementById('searchResults').innerHTML = '';
        document.getElementById('searchPatientInput').value = '';
        
        // Show success message
        showToast('success', `Đã chọn bệnh nhân: ${patientName}`);
    };

    // Show toast notification
    function showToast(type, message) {
        // Create toast element
        const toastHtml = `
            <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        // Add toast to container
        let toastContainer = document.getElementById('toastContainer');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toastContainer';
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }
        
        toastContainer.insertAdjacentHTML('beforeend', toastHtml);
        
        // Show toast
        const toastElement = toastContainer.lastElementChild;
        const toast = new bootstrap.Toast(toastElement);
        toast.show();
        
        // Remove toast after it's hidden
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    }

    // Initialize search functionality
    document.addEventListener('DOMContentLoaded', function() {
        const searchInput = document.getElementById('searchPatientInput');
        if (searchInput) {
            // Add event listener for input
            searchInput.addEventListener('input', function() {
                // Debounce search
                clearTimeout(this.searchTimeout);
                this.searchTimeout = setTimeout(() => {
                    searchPatients();
                }, 300);
            });
            
            // Add event listener for enter key
            searchInput.addEventListener('keypress', function(e) {
                if (e.key === 'Enter') {
                    searchPatients();
                }
            });
        }
    });
})(); 