document.addEventListener('DOMContentLoaded', function () {

    // Get the buttons
    const timeBasedButton = document.getElementById('timeBasedButton');
    const jobBasedButton = document.getElementById('jobBasedButton');

    // Get the divs for each content type
    const timeBasedContent = document.querySelector('.time-based-content');
    const jobBasedContent = document.querySelector('.job-based-content');

    // Add event listener for Time Based button
    timeBasedButton.addEventListener('click', function () {
        // Show Time-Based content and hide Job-Based content
        timeBasedContent.style.display = 'block';
        jobBasedContent.style.display = 'none';
    });

    // Add event listener for Job Based button
    jobBasedButton.addEventListener('click', function () {
        // Show Job-Based content and hide Time-Based content
        timeBasedContent.style.display = 'none';
        jobBasedContent.style.display = 'block';
    });
});
$(document).ready(function () {
    const initialData = [
        {
            recid: 1,
            rosterId: 12345,
            attendanceDate: '1/1/2022',
            startTime: '08:00',
            endTime: '17:00',
            totalHours: 8.00,
            breakTime: 1.00,
            pay: 100.00,
            attendanceSignPic: '', // No image initially
            blockId: 2,
            lineId: 1,
            jobPaid: true,
            remarks: 'Worked the whole day',
            appliedBy: 'Manager1',
            approvedStatus: 1,
            approvedBy: 'Supervisor1',
            approvedDt: '1/2/2022'
        }
    ];

    let grid = new w2grid({
        name: 'grid',
        box: '#grid',
        show: {
            toolbar: true,
            footer: true,
            toolbarSave: false
        },
        columns: [
            { field: 'rosterId', text: 'Roster ID', size: '100px', sortable: true, resizable: true, editable: { type: 'text' }, info: true },
            { field: 'attendanceDate', text: 'Attendance Date', size: '120px', sortable: true, resizable: true, render: 'date', style: 'text-align: right', editable: { type: 'date' } },
            { field: 'startTime', text: 'Start Time', size: '100px', sortable: true, resizable: true, render: 'time', style: 'text-align: right', editable: { type: 'time' } },
            { field: 'endTime', text: 'End Time', size: '100px', sortable: true, resizable: true, render: 'time', style: 'text-align: right', editable: { type: 'time' } },
            { field: 'totalHours', text: 'Total Hours', size: '100px', sortable: true, resizable: true, render: 'int' },
            { field: 'breakTime', text: 'Break Time', size: '100px', sortable: true, resizable: true, render: 'int', editable: { type: 'float' } },
            { field: 'pay', text: 'Pay', size: '100px', sortable: true, resizable: true, render: 'money', editable: { type: 'money' } },
            {
                field: 'attendanceSignPic',
                text: 'Attendance Sign Pic',
                size: '200px',
                resizable: true,
                editable: { type: 'text' },
                render: function (record, index, column_index) {
                    if (!record.attendanceSignPic || record.attendanceSignPic === '') {
                        return ` 
                            <button class="editBtn">Edit</button>
                            <button class="viewBtn" disabled>View</button>
                        `;
                    }
                    return ` 
                        <button class="editBtn">Edit</button>
                        <button class="viewBtn">View</button>
                    `;
                }
            },
            { field: 'blockId', text: 'Block ID', size: '100px', sortable: true, resizable: true, editable: { type: 'int' } },
            { field: 'lineId', text: 'Line ID', size: '100px', sortable: true, resizable: true, editable: { type: 'int' } },
            { field: 'jobPaid', text: 'Job Paid', size: '100px', sortable: true, resizable: true, editable: { type: 'checkbox' } },
            { field: 'remarks', text: 'Remarks', size: '200px', sortable: true, resizable: true, editable: { type: 'text' } },
            { field: 'appliedBy', text: 'Applied By', size: '120px', sortable: true, resizable: true },
            { field: 'approvedStatus', text: 'Approved Status', size: '120px', sortable: true, resizable: true },
            { field: 'approvedBy', text: 'Approved By', size: '120px', sortable: true, resizable: true },
            { field: 'approvedDt', text: 'Approved Date', size: '120px', sortable: true, resizable: true, render: 'date', style: 'text-align: right' }
        ],
        toolbar: {
            items: [
                { id: 'add', type: 'button', text: 'Add Record', icon: 'w2ui-icon-plus' },
                { id: 'save', type: 'button', text: 'Save', icon: 'w2ui-icon-save' }  // Custom Save Button
            ],
            onClick(event) {
                if (event.target == 'add') {
                    let recid = grid.records.length + 1;
                    this.owner.add({ recid });
                    this.owner.scrollIntoView(recid);
                    this.owner.editField(recid, 1);
                }
                if (event.target == 'save') {
                    // Trigger the custom save logic when the "Save" button is clicked
                    saveRecords();
                }
            }
        },
        records: initialData,
        afterEditDone(index, column, event) {
            const record = w2ui['grid'].records[index];
            const field = column;

            console.log("edit");
            console.log(event);

            if (field === 2 || field === 3 || field === 5) {
                // Calculate total hours after editing start or end time
                calculateTotalHours(record, field);
            }
            //w2ui['grid'].refresh();
        }
    });

    // Custom save function
    function saveRecords() {
        const grid = w2ui['grid'];
        grid.save();
        const recordsToSave = grid.records;

        // Validation for empty required fields
        let invalidRecord = null;
        for (let record of recordsToSave) {
            if (!record.rosterId || !record.attendanceDate || !record.startTime || !record.endTime) {
                invalidRecord = record;
                break;
            }
        }

        if (invalidRecord) {
            alert('Roster ID, Attendance Date, Start Time, and End Time cannot be empty.');
            return;
        }

        let attendanceFormData = recordsToSave.map(record => ({
            RosterId: record.rosterId,
            AttendanceDate: new Date(record.attendanceDate).toISOString(),  // Ensure it is an ISO string
            StartTime: record.startTime,
            EndTime: record.endTime,
            TotalHours: record.totalHours,
            BreakTime: record.breakTime,
            Pay: record.pay,
            AttendanceSignPic: record.attendanceSignPic.substr(23) || '',
            BlockId: record.blockId,
            LineId: record.lineId,
            JobPaid: record.jobPaid == true? "Y" : "N",
            Remarks: record.remarks,
            AppliedBy: record.appliedBy,
            ApprovedStatus: "P",
            ApprovedBy: record.approvedBy,
            ApprovedDt: new Date(record.approvedDt).toISOString()  // Ensure it is an ISO string
        }));

        // Utility function to format time
        function formatTime(time) {
            if (time) {
                let date = new Date("1970-01-01T" + time + "Z");
                return date.toISOString().substr(11, 8); // Convert to "hh:mm:ss" format
            }
            return null;
        }

        let rootUrl = window.location.origin;

        $.ajax({
            url: '/Attendance/SaveTimeAttendance',  // Your controller action URL
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(attendanceFormData),  // Sending the data as JSON
            success: function (response) {
                if (response.Success) {
                    alert("Attendance records saved successfully!");
                    window.location.reload();
                } else {
                    alert("Failed to save attendance: " + response.Message);
                }
            },
            error: function (xhr, status, error) {
                alert("Error: " + error + "\nStatus: " + status);
            }
        });

    }




    // Calculate total hours
    function calculateTotalHours(record, field) {
        const startTime = (record.w2ui?.changes?.startTime
            ? moment(record.w2ui.changes.startTime, 'HH:mm')
            : moment(record.startTime, 'HH:mm'));
        const endTime = (record.w2ui?.changes?.endTime
            ? moment(record.w2ui.changes.endTime, 'HH:mm')
            : moment(record.endTime, 'HH:mm'));
        const breakTime = ((record.w2ui?.changes?.breakTime ? record.w2ui.changes.breakTime : record.breakTime)) || 0;

        if (startTime && endTime) {
            const start = moment(startTime, 'HH:mm');
            const end = moment(endTime, 'HH:mm');
            console.log(start);
            console.log(end);
            let totalHours = end.diff(start, 'hours', true); // Total hours as a decimal
            if (totalHours < 0) {
                totalHours += 24; // If the end time is after midnight, adjust by adding 24 hours
            }
            console.log(totalHours);
            record.w2ui = record.w2ui ?? {};
            record.w2ui.changes = record.w2ui.changes ?? {};
            record.w2ui.changes.totalHours = totalHours - breakTime; // Subtract break time
            
            grid.refresh();
        }
    }

    // Trigger file input when clicking the 'E' button (Edit)
    $(document).on('click', '.editBtn', function (event) {
        const recid = $(this).closest('tr').attr('recid');
        const grid = w2ui['grid'];
        const record = grid.get(recid);

        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.style.display = 'none';

        input.addEventListener('change', function (e) {
            if (e.target.files.length > 0) {
                var file = e.target.files[0];
                var reader = new FileReader();
                reader.onload = function (event) {
                    var base64Image = event.target.result;
                    record.attendanceSignPic = base64Image;
                    grid.refresh();
                };
                reader.readAsDataURL(file);
            }
        });

        input.click();
    });

    // View image on clicking the 'V' button (View)
    $(document).on('click', '.viewBtn', function (event) {
        const recid = $(this).closest('tr').attr('recid');
        const grid = w2ui['grid'];
        const record = grid.get(recid);

        if (!record.attendanceSignPic || record.attendanceSignPic === '') {
            alert('No image available');
            return;
        }

        w2popup.open({
            title: 'Attendance Sign Pic',
            width: 400,
            height: 400,
            body: `<img src="${record.attendanceSignPic}" style="width:100%; height:auto;" />`,
            actions: { Ok: w2popup.close }
        });
    });

    // Reload grid data logic
    $('#tb_grid_toolbar_item_w2ui-reload').on('click', function () {
        grid.clear();
        grid.add(initialData);
        grid.refresh();
    });
});

