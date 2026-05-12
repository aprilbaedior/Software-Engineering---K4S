/**
 * Attendance Management System
 * 
 * This JavaScript file provides the interactive functionality for the attendance management interface.
 * It handles attendance updates, student summaries, class statistics, bulk operations, and more.
 */

/**
 * Submits an attendance update for a single student and date.
 * @param {HTMLElement} selectElement - The select dropdown element that triggered the update
 */
async function submitAttendance(selectElement) {
    const form = selectElement.closest("form");
    const formData = new FormData(form);

    try {
        const response = await fetch(form.action, {
            method: "POST",
            body: formData
        });

        const result = await response.json();
        if (result.success) {
            console.log(result.message);
            updateDropdownStyle(selectElement);
            updateStudentSummary(form.querySelector("[name='email']").value);
            updateClassStats();
        } else {
            console.error("Failed to update attendance:", result.message || "Unknown error");
        }
    } catch (error) {
        console.error("Error updating attendance:", error);
    }
}

/**
 * Updates the styling of an attendance dropdown based on its selected value.
 * @param {HTMLElement} selectElement - The select dropdown element to style
 */
function updateDropdownStyle(selectElement) {
    // Remove all status classes
    selectElement.classList.remove(
        "status-present", 
        "status-absent", 
        "status-late", 
        "status-excused", 
        "status-not-marked"
    );

    // Add the appropriate class based on the current value
    const status = selectElement.value.toLowerCase().replace(" ", "-");
    selectElement.classList.add(`status-${status}`);
}

/**
 * Calculates and updates the attendance summary for a specific student.
 * @param {string} email - The email of the student
 */
function updateStudentSummary(email) {
    const studentRow = document.querySelector(`.student-row[data-email="${email}"]`);
    if (!studentRow) return;

    const attendanceCells = studentRow.querySelectorAll('.attendance-cell .status-dropdown');
    let presentCount = 0;
    let absentCount = 0;
    let lateCount = 0;
    let excusedCount = 0;
    let totalMarked = 0;

    attendanceCells.forEach(dropdown => {
        const status = dropdown.value;
        if (status !== "Not Marked") {
            totalMarked++;
            
            if (status === "Present") presentCount++;
            else if (status === "Absent") absentCount++;
            else if (status === "Late") lateCount++;
            else if (status === "Excused") excusedCount++;
        }
    });

    if (totalMarked > 0) {
        const presentWidth = (presentCount / totalMarked) * 100;
        const lateWidth = (lateCount / totalMarked) * 100;
        const excusedWidth = (excusedCount / totalMarked) * 100;
        const absentWidth = (absentCount / totalMarked) * 100;

        const summaryBar = studentRow.querySelector('.attendance-summary-bar');
        if (summaryBar) {
            const presentSegment = summaryBar.querySelector('.present');
            const lateSegment = summaryBar.querySelector('.late');
            const excusedSegment = summaryBar.querySelector('.excused');
            const absentSegment = summaryBar.querySelector('.absent');

            if (presentSegment) {
                presentSegment.style.width = `${presentWidth}%`;
                presentSegment.setAttribute('data-count', presentCount);
            }
            
            if (lateSegment) {
                lateSegment.style.width = `${lateWidth}%`;
                lateSegment.setAttribute('data-count', lateCount);
            }
            
            if (excusedSegment) {
                excusedSegment.style.width = `${excusedWidth}%`;
                excusedSegment.setAttribute('data-count', excusedCount);
            }
            
            if (absentSegment) {
                absentSegment.style.width = `${absentWidth}%`;
                absentSegment.setAttribute('data-count', absentCount);
            }

            summaryBar.title = `Present: ${presentCount}, Late: ${lateCount}, Excused: ${excusedCount}, Absent: ${absentCount}`;
        }
    }
}

/**
 * Updates the class-wide attendance statistics.
 */
function updateClassStats() {
    let presentTotal = 0;
    let absentTotal = 0;
    let lateTotal = 0;
    let excusedTotal = 0;

    document.querySelectorAll('.status-dropdown').forEach(dropdown => {
        const status = dropdown.value;
        if (status === "Present") presentTotal++;
        else if (status === "Absent") absentTotal++;
        else if (status === "Late") lateTotal++;
        else if (status === "Excused") excusedTotal++;
    });

    const presentCountElem = document.getElementById('presentCount');
    const absentCountElem = document.getElementById('absentCount');
    const lateCountElem = document.getElementById('lateCount');
    
    if (presentCountElem) presentCountElem.textContent = presentTotal;
    if (absentCountElem) absentCountElem.textContent = absentTotal;
    if (lateCountElem) lateCountElem.textContent = lateTotal;
}

/**
 * Opens the detailed student modal with attendance metrics.
 * @param {string} email - The email of the student
 */
function openStudentModal(email) {
    const modal = document.getElementById('studentModal');
    const modalTitle = document.getElementById('modalStudentEmail');
    
    if (modal && modalTitle) {
        modalTitle.textContent = email;
        modal.style.display = 'block';
        
        populateStudentMetrics(email);
    }
}

/**
 * Populates student attendance metrics in the detail modal.
 * @param {string} email - The email of the student
 */
function populateStudentMetrics(email) {
    const studentRow = document.querySelector(`.student-row[data-email="${email}"]`);
    if (!studentRow) return;

    const attendanceCells = studentRow.querySelectorAll('.attendance-cell .status-dropdown');
    let presentCount = 0;
    let absentCount = 0;
    let lateCount = 0;
    let excusedCount = 0;
    let totalMarked = 0;
    
    const history = [];

    attendanceCells.forEach(dropdown => {
        const status = dropdown.value;
        if (status !== "Not Marked") {
            totalMarked++;
            
            if (status === "Present") presentCount++;
            else if (status === "Absent") absentCount++;
            else if (status === "Late") lateCount++;
            else if (status === "Excused") excusedCount++;
            
            // Add to history
            const dateString = dropdown.closest('form').querySelector('[name="date"]').value;
            if (dateString) {
                history.push({
                    date: new Date(dateString),
                    status: status
                });
            }
        }
    });

    history.sort((a, b) => a.date - b.date);

    // Update metrics
    const studentPresentElem = document.getElementById('studentPresent');
    const studentAbsentElem = document.getElementById('studentAbsent');
    const studentLateElem = document.getElementById('studentLate');
    const studentExcusedElem = document.getElementById('studentExcused');
    const studentAttendanceElem = document.getElementById('studentAttendance');
    
    if (studentPresentElem) studentPresentElem.textContent = presentCount;
    if (studentAbsentElem) studentAbsentElem.textContent = absentCount;
    if (studentLateElem) studentLateElem.textContent = lateCount;
    if (studentExcusedElem) studentExcusedElem.textContent = excusedCount;

    if (totalMarked > 0 && studentAttendanceElem) {
        const weightedSum = 
            (presentCount * 1.0) + 
            (lateCount * 0.8) + 
            (excusedCount * 0.5) + 
            (absentCount * 0.0);
        
        const attendanceScore = (weightedSum / totalMarked) * 100;
        studentAttendanceElem.textContent = attendanceScore.toFixed(1) + '%';
    } else if (studentAttendanceElem) {
        studentAttendanceElem.textContent = 'N/A';
    }

    generateTrendChart(history);
    generateHistoryList(history);
}

/**
 * Generates a visual trend chart for student attendance.
 * @param {Array} history - Array of attendance history objects
 */
function generateTrendChart(history) {
    const trendLine = document.querySelector('.trend-line');
    if (!trendLine) return;
    
    trendLine.innerHTML = '';

    history.forEach(entry => {
        const trendDay = document.createElement('div');
        trendDay.className = 'trend-day';
        
        const trendBar = document.createElement('div');
        trendBar.className = `trend-bar ${entry.status.toLowerCase()}`;
        
        let height = '20%';
        if (entry.status === 'Present') height = '100%';
        else if (entry.status === 'Late') height = '80%';
        else if (entry.status === 'Excused') height = '50%';
        else if (entry.status === 'Absent') height = '20%';
        
        trendBar.style.height = height;
        
        const trendDate = document.createElement('span');
        trendDate.className = 'trend-date';
        trendDate.textContent = entry.date.toLocaleDateString('en-US', { month: 'numeric', day: 'numeric' });
        
        trendDay.appendChild(trendBar);
        trendDay.appendChild(trendDate);
        trendLine.appendChild(trendDay);
    });
}

/**
 * Generates a history list for student attendance.
 * @param {Array} history - Array of attendance history objects
 */
function generateHistoryList(history) {
    const historyList = document.getElementById('studentHistory');
    if (!historyList) return;
    
    historyList.innerHTML = '';

    [...history].reverse().forEach(entry => {
        const listItem = document.createElement('li');
        
        const dateSpan = document.createElement('span');
        dateSpan.className = 'history-date';
        dateSpan.textContent = entry.date.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' }) + ' - ';
        
        const statusSpan = document.createElement('span');
        statusSpan.className = `history-status ${entry.status.toLowerCase()}`;
        statusSpan.textContent = entry.status;
        
        listItem.appendChild(dateSpan);
        listItem.appendChild(statusSpan);
        historyList.appendChild(listItem);
    });
}

/**
 * Filters students based on attendance criteria.
 * @param {string} filterType - Type of filter to apply (all, perfect, absent, late, unmarked)
 */
function filterStudents(filterType) {
    const studentRows = document.querySelectorAll('.student-row');
    
    studentRows.forEach(row => {
        if (filterType === 'all') {
            row.style.display = '';
            return;
        }
        
        const dropdowns = row.querySelectorAll('.status-dropdown');
        let hasAbsence = false;
        let hasLate = false;
        let hasUnmarked = false;
        let allPresent = true;
        
        dropdowns.forEach(dropdown => {
            const status = dropdown.value;
            if (status === 'Absent') hasAbsence = true;
            if (status === 'Late') hasLate = true;
            if (status === 'Not Marked') hasUnmarked = true;
            if (status !== 'Present') allPresent = false;
        });
        
        if (filterType === 'perfect' && allPresent) {
            row.style.display = '';
        } else if (filterType === 'absent' && hasAbsence) {
            row.style.display = '';
        } else if (filterType === 'late' && hasLate) {
            row.style.display = '';
        } else if (filterType === 'unmarked' && hasUnmarked) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

/**
 * Exports attendance data to a CSV file.
 */
function exportToCSV() {
    const rows = [];
    const headers = ['Student Name'];
    
    document.querySelectorAll('.date-column').forEach(col => {
        if (col.dataset.date) {
            headers.push(col.dataset.date);
        }
    });
    
    headers.push('Present Count', 'Late Count', 'Excused Count', 'Absent Count');
    rows.push(headers);
    
    document.querySelectorAll('.student-row').forEach(row => {
        if (!row.dataset.email) return;
        
        const studentData = [row.dataset.email];
        
        row.querySelectorAll('.attendance-cell .status-dropdown').forEach(dropdown => {
            studentData.push(dropdown.value);
        });
        
        const summaryBar = row.querySelector('.attendance-summary-bar');
        if (summaryBar) {
            const presentSegment = summaryBar.querySelector('.present');
            const lateSegment = summaryBar.querySelector('.late');
            const excusedSegment = summaryBar.querySelector('.excused');
            const absentSegment = summaryBar.querySelector('.absent');
            
            studentData.push(
                presentSegment ? presentSegment.dataset.count || '0' : '0',
                lateSegment ? lateSegment.dataset.count || '0' : '0',
                excusedSegment ? excusedSegment.dataset.count || '0' : '0',
                absentSegment ? absentSegment.dataset.count || '0' : '0'
            );
        }
        
        rows.push(studentData);
    });
    
    const csvContent = rows.map(row => row.join(',')).join('\n');
    
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'attendance_report.csv';
    link.click();
}

/**
 * Get array of selected student emails for bulk operations.
 * @returns {Array} Array of selected student emails
 */
function getBulkSelectedStudents() {
    const selectedCheckboxes = document.querySelectorAll('.student-checkbox:checked');
    return Array.from(selectedCheckboxes).map(checkbox => 
        checkbox.closest('.student-row').dataset.email
    );
}

/**
 * Get array of selected dates for bulk operations.
 * @returns {Array} Array of selected dates
 */
function getSelectedDates() {
    const selectedDateCheckboxes = document.querySelectorAll('.date-checkbox:checked');
    return Array.from(selectedDateCheckboxes).map(checkbox => 
        checkbox.closest('.date-option').dataset.date
    );
}

/**
 * Apply bulk attendance changes to multiple students across multiple dates.
 * @param {string} status - The attendance status to set
 * @param {Array} dates - Array of date strings
 * @param {Array} emails - Array of student emails
 * @returns {Promise} A promise that resolves when all operations are complete
 */
async function applyBulkAttendance(status, dates, emails) {
    if (!dates.length || !emails.length) {
        throw new Error("No dates or emails selected for bulk update");
    }
    
    document.body.style.cursor = 'wait';
    
    try {
        // Process updates in batches to avoid overwhelming the server
        const batchSize = 5; // Process 5 updates at a time
        const results = [];
        
        // Create an array of all the updates we need to make
        const updates = [];
        for (const email of emails) {
            for (const date of dates) {
                updates.push({ email, date });
            }
        }
        
        // Process updates in batches
        for (let i = 0; i < updates.length; i += batchSize) {
            const batch = updates.slice(i, i + batchSize);
            const batchPromises = batch.map(update => {
                return processAttendanceUpdate(update.email, update.date, status);
            });
            
            // Wait for the current batch to complete
            const batchResults = await Promise.allSettled(batchPromises);
            results.push(...batchResults);
        }
        
        // Count successful updates
        const successfulUpdates = results.filter(r => r.status === 'fulfilled').length;
        const failedUpdates = results.filter(r => r.status === 'rejected').length;
        
        console.log(`Successfully updated ${successfulUpdates} records, ${failedUpdates} failed`);
        
        // Update UI for all successful updates
        if (successfulUpdates > 0) {
            updateUIAfterBulkChange(emails, dates, status);
        }
        
        // If there were failures, throw an error
        if (failedUpdates > 0) {
            throw new Error(`Failed to update ${failedUpdates} of ${results.length} records`);
        }
        
        return { success: true, message: `Successfully updated ${successfulUpdates} attendance records` };
    } catch (error) {
        console.error("Error in applyBulkAttendance:", error);
        throw error;
    } finally {
        document.body.style.cursor = 'default';
    }
}

/**
 * Process a single attendance update with proper error handling.
 * @param {string} email - Student email
 * @param {string} date - Date string
 * @param {string} status - Attendance status
 * @returns {Promise} A promise that resolves with the update result
 */
async function processAttendanceUpdate(email, date, status) {
    // Find the form for this student and date
    const formSelector = `.student-row[data-email="${email}"] .attendance-cell[data-date="${date}"] form`;
    const form = document.querySelector(formSelector);
    
    if (!form) {
        console.warn(`No form found for ${email} on date ${date}`);
        return { success: false, message: 'Form not found' };
    }
    
    try {
        // Create a new FormData object from the form
        const formData = new FormData(form);
        
        // Update the status in the form data
        formData.set('status', status);
        
        // Make the fetch request with proper error handling
        const response = await fetch(form.action, {
            method: "POST",
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error(`HTTP error ${response.status} for ${email} on ${date}: ${errorText}`);
            throw new Error(`HTTP error: ${response.status}`);
        }
        
        // Try to parse the response as JSON
        let data;
        try {
            data = await response.json();
        } catch (e) {
            console.warn(`Response wasn't JSON for ${email} on ${date}`);
            // If we can't parse as JSON but response was OK, still consider it a success
            return { success: true, message: 'Update successful but no JSON response' };
        }
        
        return { success: true, data };
    } catch (error) {
        console.error(`Error updating attendance for ${email} on ${date}:`, error);
        throw error;
    }
}

/**
 * Update the UI after a successful bulk change.
 * @param {Array} emails - Array of student emails
 * @param {Array} dates - Array of date strings
 * @param {string} status - Attendance status that was set
 */
function updateUIAfterBulkChange(emails, dates, status) {
    // Update dropdowns
    emails.forEach(email => {
        dates.forEach(date => {
            const dropdownSelector = `.student-row[data-email="${email}"] .attendance-cell[data-date="${date}"] select`;
            const dropdown = document.querySelector(dropdownSelector);
            
            if (dropdown) {
                dropdown.value = status;
                updateDropdownStyle(dropdown);
            }
        });
        
        // Update the summary for each student
        updateStudentSummary(email);
    });
    
    // Update the overall class statistics
    updateClassStats();
}

/**
 * Load available students for enrollment.
 * @param {string} query - Optional search query
 */
async function loadAvailableStudents(query = '') {
    const sectionID = document.querySelector('[name="sectionID"]').value;
    try {
        console.log(`Fetching students for section ${sectionID}`);
        const response = await fetch(`/api/StudentSearch?sectionID=${sectionID}&query=${encodeURIComponent(query || '')}`);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error(`HTTP error! Status: ${response.status}, Response: ${errorText}`);
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        
        const students = await response.json();
        console.log(`Loaded ${students.length} students`, students);
        
        const searchResults = document.getElementById('searchResults');
        if (!searchResults) return;
        
        searchResults.innerHTML = '';
        
        if (students.length === 0) {
            searchResults.innerHTML = '<div class="no-students-message">No available students found</div>';
            return;
        }
        
        students.forEach(student => {
            const item = document.createElement('div');
            item.className = 'search-result-item';
            item.textContent = `${student.name} (${student.email})`;
            item.dataset.email = student.email;
            item.dataset.name = student.name;
            
            item.addEventListener('click', () => {
                const emailInput = document.getElementById('selectedStudentEmail');
                const selectedDiv = document.getElementById('selectedStudent');
                const container = document.getElementById('selectedStudentContainer');
                
                if (emailInput) emailInput.value = student.email;
                if (selectedDiv) selectedDiv.textContent = `${student.name} (${student.email})`;
                if (container) container.style.display = 'block';
            });
            
            searchResults.appendChild(item);
        });
    } catch (error) {
        console.error('Error loading available students:', error);
        const searchResults = document.getElementById('searchResults');
        if (searchResults) {
            searchResults.innerHTML = 
                '<div class="no-students-message">Error loading students. Please try again.</div>';
        }
    }
}

/**
 * Initialize all event listeners and UI elements when the DOM is loaded.
 */
document.addEventListener("DOMContentLoaded", function() {
    // Initialize all select dropdowns
    document.querySelectorAll(".status-dropdown").forEach(select => {
        updateDropdownStyle(select);
    });
    
    // Update all student summaries
    document.querySelectorAll(".student-row").forEach(row => {
        updateStudentSummary(row.dataset.email);
    });
    
    // Update class stats
    updateClassStats();
    
    // Student info modal
    document.querySelectorAll('.student-info-icon').forEach(icon => {
        icon.addEventListener('click', () => {
            openStudentModal(icon.dataset.email);
        });
    });
    
    // Close student modal
    const closeModalBtn = document.querySelector('#studentModal .close-modal');
    if (closeModalBtn) {
        closeModalBtn.addEventListener('click', () => {
            document.getElementById('studentModal').style.display = 'none';
        });
    }
    
    // Close modals when clicking outside
    window.addEventListener('click', event => {
        const studentModal = document.getElementById('studentModal');
        if (event.target === studentModal) {
            studentModal.style.display = 'none';
        }
        
        const enrollModal = document.getElementById('enrollStudentModal');
        if (event.target === enrollModal) {
            enrollModal.style.display = 'none';
        }
    });
    
    // Filter dropdown functionality
    document.querySelectorAll('.filter-content a').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            filterStudents(link.dataset.filter);
        });
    });
    
    // Export button
    const exportBtn = document.getElementById('exportBtn');
    if (exportBtn) {
        exportBtn.addEventListener('click', exportToCSV);
    }
    
    // Select all students checkbox
    const selectAllCheckbox = document.getElementById('selectAll');
    if (selectAllCheckbox) {
        selectAllCheckbox.addEventListener('change', function() {
            const isChecked = this.checked;
            document.querySelectorAll('.student-checkbox').forEach(checkbox => {
                checkbox.checked = isChecked;
            });
            
            const bulkActionBtn = document.getElementById('bulkActionBtn');
            const applyDateBtn = document.getElementById('applyDateBtn');
            
            if (bulkActionBtn) bulkActionBtn.disabled = !isChecked;
            if (applyDateBtn) applyDateBtn.disabled = !isChecked;
        });
    }
    
    // Individual student checkboxes
    document.querySelectorAll('.student-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', () => {
            const anyChecked = document.querySelector('.student-checkbox:checked') !== null;
            const bulkActionBtn = document.getElementById('bulkActionBtn');
            const applyDateBtn = document.getElementById('applyDateBtn');
            
            if (bulkActionBtn) bulkActionBtn.disabled = !anyChecked;
            if (applyDateBtn) applyDateBtn.disabled = !anyChecked;
            
            const allChecked = document.querySelectorAll('.student-checkbox:not(:checked)').length === 0;
            const selectAllCheckbox = document.getElementById('selectAll');
            
            if (selectAllCheckbox) selectAllCheckbox.checked = allChecked;
        });
    });
    
    // Bulk action dropdown
    document.querySelectorAll('.bulk-content a').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const action = link.dataset.action;
            console.log(`Bulk action clicked: ${action}`);
            
            const selectedStudents = getBulkSelectedStudents();
            
            if (selectedStudents.length === 0) {
                alert('Please select at least one student.');
                return;
            }
            
            // Store the selected action in a data attribute for later use
            const applyToSelectedDatesBtn = document.getElementById('applyToSelectedDates');
            if (applyToSelectedDatesBtn) {
                applyToSelectedDatesBtn.dataset.actionType = action;
            }
            
            // Position the date selector near the button
            const bulkBtn = document.getElementById('bulkActionBtn');
            const dateSelector = document.getElementById('dateSelector');
            
            if (!bulkBtn || !dateSelector) {
                console.error('Missing elements: bulkBtn or dateSelector');
                return;
            }
            
            const rect = bulkBtn.getBoundingClientRect();
            
            dateSelector.style.position = 'absolute';
            dateSelector.style.top = (rect.bottom + window.scrollY + 5) + 'px';
            dateSelector.style.left = (rect.left + window.scrollX) + 'px';
            dateSelector.style.display = 'block';
            
            // Add a visible indicator of what action is selected
            const actionIndicator = document.createElement('div');
            actionIndicator.id = 'action-indicator';
            actionIndicator.style.padding = '5px';
            actionIndicator.style.marginBottom = '10px';
            actionIndicator.style.backgroundColor = '#f0f8ff';
            actionIndicator.style.border = '1px solid #add8e6';
            actionIndicator.style.borderRadius = '3px';
            actionIndicator.style.fontSize = '14px';
            actionIndicator.textContent = `Selected action: ${action}`;
            
            // Remove any existing indicator
            const existingIndicator = dateSelector.querySelector('#action-indicator');
            if (existingIndicator) {
                existingIndicator.remove();
            }
            
            // Add the new indicator at the top of the date selector
            const header = dateSelector.querySelector('.date-selector-header');
            if (header && header.parentNode) {
                header.parentNode.insertBefore(actionIndicator, header.nextSibling);
            }
        });
    });

    // Apply to date button
    const applyDateBtn = document.getElementById('applyDateBtn');
    if (applyDateBtn) {
        applyDateBtn.addEventListener('click', () => {
            const selectedStudents = getBulkSelectedStudents();
            
            if (selectedStudents.length === 0) {
                alert('Please select at least one student.');
                return;
            }
            
            // Position the date selector near the button
            const dateSelector = document.getElementById('dateSelector');
            
            if (!applyDateBtn || !dateSelector) {
                console.error('Missing elements: applyDateBtn or dateSelector');
                return;
            }
            
            const rect = applyDateBtn.getBoundingClientRect();
            
            dateSelector.style.position = 'absolute';
            dateSelector.style.top = (rect.bottom + window.scrollY + 5) + 'px';
            dateSelector.style.left = (rect.left + window.scrollX) + 'px';
            dateSelector.style.display = 'block';
            
            // When clicking Apply to Date, we should clear any stored action type
            // as this flow doesn't specify an action yet
            const applyToSelectedDatesBtn = document.getElementById('applyToSelectedDates');
            if (applyToSelectedDatesBtn) {
                applyToSelectedDatesBtn.dataset.actionType = '';
            }
            
            // Remove any existing indicator
            const existingIndicator = dateSelector.querySelector('#action-indicator');
            if (existingIndicator) {
                existingIndicator.remove();
            }
            
            // Add an indicator that we need to select an action
            const actionIndicator = document.createElement('div');
            actionIndicator.id = 'action-indicator';
            actionIndicator.style.padding = '5px';
            actionIndicator.style.marginBottom = '10px';
            actionIndicator.style.backgroundColor = '#fff3cd';
            actionIndicator.style.border = '1px solid #ffeeba';
            actionIndicator.style.borderRadius = '3px';
            actionIndicator.style.fontSize = '14px';
            actionIndicator.textContent = 'Please select an action after choosing dates';
            
            const header = dateSelector.querySelector('.date-selector-header');
            if (header && header.parentNode) {
                header.parentNode.insertBefore(actionIndicator, header.nextSibling);
            }
        });
    }

    // Close date selector
    const closeDateSelectorBtn = document.getElementById('closeDateSelector');
    if (closeDateSelectorBtn) {
        closeDateSelectorBtn.addEventListener('click', () => {
            const dateSelector = document.getElementById('dateSelector');
            if (dateSelector) {
                dateSelector.style.display = 'none';
            }
        });
    }

    // Apply to selected dates
    const applyToSelectedDatesBtn = document.getElementById('applyToSelectedDates');
    if (applyToSelectedDatesBtn) {
        applyToSelectedDatesBtn.addEventListener('click', async () => {
            const selectedDates = getSelectedDates();
            
            if (selectedDates.length === 0) {
                alert('Please select at least one date.');
                return;
            }
            
            const selectedStudents = getBulkSelectedStudents();
            
            // Get the action type from the data attribute
            const actionType = applyToSelectedDatesBtn.dataset.actionType || '';
            
            if (!actionType) {
                alert('Please select an action (Present, Absent, Late, etc.) before applying.');
                return;
            }
            
            // Add a visual indicator that processing is happening
            const dateSelector = document.getElementById('dateSelector');
            if (dateSelector) {
                const processingIndicator = document.createElement('div');
                processingIndicator.id = 'processing-indicator';
                processingIndicator.style.position = 'absolute';
                processingIndicator.style.top = '0';
                processingIndicator.style.left = '0';
                processingIndicator.style.width = '100%';
                processingIndicator.style.height = '100%';
                processingIndicator.style.backgroundColor = 'rgba(255, 255, 255, 0.8)';
                processingIndicator.style.display = 'flex';
                processingIndicator.style.justifyContent = 'center';
                processingIndicator.style.alignItems = 'center';
                processingIndicator.style.zIndex = '10001';
                processingIndicator.innerHTML = '<div style="background-color: #f8f9fa; padding: 15px; border-radius: 5px; box-shadow: 0 0 10px rgba(0,0,0,0.1);">Processing...</div>';
                
                dateSelector.appendChild(processingIndicator);
            }
            
            try {
                // Use the improved bulk attendance function
                await applyBulkAttendance(actionType, selectedDates, selectedStudents);
                
                // Display success indicator before hiding
                if (dateSelector) {
                    const processingIndicator = document.getElementById('processing-indicator');
                    if (processingIndicator) {
                        processingIndicator.innerHTML = '<div style="background-color: #d4edda; padding: 15px; border-radius: 5px; box-shadow: 0 0 10px rgba(0,0,0,0.1);">Update Successful!</div>';
                        
                        // Hide after a short delay
                        setTimeout(() => {
                            if (dateSelector) dateSelector.style.display = 'none';
                            if (processingIndicator) processingIndicator.remove();
                        }, 1000);
                    } else {
                        dateSelector.style.display = 'none';
                    }
                }
                
                // Uncheck the date checkboxes after applying
                document.querySelectorAll('.date-checkbox').forEach(checkbox => {
                    checkbox.checked = false;
                });
            } catch (error) {
                console.error('Error in bulk update:', error);
                
                // Display error indicator
                if (dateSelector) {
                    const processingIndicator = document.getElementById('processing-indicator');
                    if (processingIndicator) {
                        processingIndicator.innerHTML = `<div style="background-color: #f8d7da; padding: 15px; border-radius: 5px; box-shadow: 0 0 10px rgba(0,0,0,0.1);">Error: ${error.message || 'Unknown error'}</div>`;
                        
                        // Hide after a short delay
                        setTimeout(() => {
                            if (processingIndicator) processingIndicator.remove();
                        }, 3000);
                    }
                }
            }
        });
    }
    
    // Enroll student button
    const enrollStudentBtn = document.getElementById('enrollStudentBtn');
    if (enrollStudentBtn) {
        enrollStudentBtn.addEventListener('click', () => {
            const enrollModal = document.getElementById('enrollStudentModal');
            const selectedStudentContainer = document.getElementById('selectedStudentContainer');
            const selectedStudentEmail = document.getElementById('selectedStudentEmail');
            
            if (enrollModal) enrollModal.style.display = 'block';
            if (selectedStudentContainer) selectedStudentContainer.style.display = 'none';
            if (selectedStudentEmail) selectedStudentEmail.value = '';
            
            // Load available students
            loadAvailableStudents();
        });
    }
    
    // Handle search button click
    const searchButton = document.getElementById('searchButton');
    if (searchButton) {
        searchButton.addEventListener('click', () => {
            const query = document.getElementById('studentSearch').value;
            loadAvailableStudents(query);
        });
    }
    
    // Handle search input when user presses Enter
    const studentSearchInput = document.getElementById('studentSearch');
    if (studentSearchInput) {
        studentSearchInput.addEventListener('keypress', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                const query = studentSearchInput.value;
                loadAvailableStudents(query);
            }
        });
    }
    
    // Close enroll modal
    const closeEnrollModal = document.getElementById('closeEnrollModal');
    if (closeEnrollModal) {
        closeEnrollModal.addEventListener('click', () => {
            const enrollModal = document.getElementById('enrollStudentModal');
            if (enrollModal) enrollModal.style.display = 'none';
        });
    }
    
    // Cancel enrollment
    const cancelEnrollment = document.getElementById('cancelEnrollment');
    if (cancelEnrollment) {
        cancelEnrollment.addEventListener('click', () => {
            const enrollModal = document.getElementById('enrollStudentModal');
            if (enrollModal) enrollModal.style.display = 'none';
        });
    }
});