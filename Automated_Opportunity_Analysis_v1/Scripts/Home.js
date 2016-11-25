$(function () {
    var serverURL = "";

    $(document).ready(function () {
        $('#DBDropDown').spinner;

       

        $.ajax({
            type: "GET",
            url: serverURL + "Home.aspx/GetDBList",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                var source = document.getElementById("dbTemplate").innerHTML;
                var template = Handlebars.compile(source);
                document.getElementById("DBDropDown").innerHTML = template({ rows: JSON.parse(response.d) });
                $('#DBDropDown').trigger('change');
            }
        });    
            
    });

    $('#DBDropDown').change(function () {
        var databaseSelected = $(this).find("option:selected").text();

        $.ajax({
            type: "GET",
            url: serverURL + "Home.aspx/GetHosList",
            contentType: "application/json; charset=utf-8",
            data: { databaseName: "'" + databaseSelected + "'"},
            dataType: "json",
            success: function (data) {

                var source = document.getElementById("hosTemplate").innerHTML;
                var template = Handlebars.compile(source);

                var rows = JSON.parse(data.d);
                var optionEl = "";
                for (var i = 0; i < rows.length; i++) {
                    optionEl += "<option  value='" + rows[i].HospitalName + "'>" + rows[i].HospitalName + "</option>";
                }
                if ($('#example-enableFiltering-includeSelectAllOption').data('crux-selectr')) {
                    $('#example-enableFiltering-includeSelectAllOption').selectr('destroy');
                    $('#example-enableFiltering-includeSelectAllOption').show();
                }

                $('#example-enableFiltering-includeSelectAllOption').html(optionEl);
                $('#example-enableFiltering-includeSelectAllOption').selectr({
                    width: 350,
                    listwidth: 350
                });
                //document.getElementById("example-enableFiltering-includeSelectAllOption").innerHTML = template({ rows: JSON.parse(data.d) });
            },

            error: function(data,success,error){
                alert(error);
            }
        });

        

        $.ajax({
            type: "GET",
            url: serverURL + "Home.aspx/GetMaxDischargeDate",
            contentType: "application/json; charset=utf-8",
            data: { databaseName: "'" + databaseSelected + "'"},
            dataType: "json",
            success: function (data) {

                var source = document.getElementById("maxdischargeTemplate").innerHTML;
                var template = Handlebars.compile(source);
                
                document.getElementById("maxdischarge").innerHTML = template({ rows: JSON.parse(data.d) });               

            },

            error: function(data,success,error){
                alert(error);
            }
        });

    });      
    
        

        $('#Submit').click(function (e) {
            //var mesg = $("#mesg")[0];

            if ($("#DBDropDown").find("option:selected").text() != "--Select--") {

                var hospitalList = "";
                var selectedHospitals = $($('#example-enableFiltering-includeSelectAllOption').selectr('html', 'list')).find(':checked');

                selectedHospitals.each(function (i, el) {
                    hospitalList += $(el).parent().find('label').text();
                    hospitalList += (i < (selectedHospitals.length - 1)) ? ',' : '';
                });

                if (hospitalList && hospitalList != "") {

                    if ($("#startdt").val() != "") {

                        if ($("#enddt").val() != "") {

                            $('.loading').show();                            

                            $.ajax({
                                type: "GET",
                                url: serverURL + 'Home.aspx/Register',
                                data: { databaseName: "'" + $('#DBDropDown').find("option:selected").text() + "'", hospital: "'" + hospitalList + "'", startdate: "'" + $('#startdt').val() + "'", enddate: "'" + $('#enddt').val() + "'", email: "'" + $('#email').val() + "'" },
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data.d == 'Success') {
                                        //mesg.style.color = "green";
                                        mesg = $.growl.notice({ message: "Your report has been successfully generated. You will recieve an email shortly." });
                                        $('.loading').hide();
                                        e.preventDefault();
                                    }
                                    else if (data.d == 'Failure') {
                                        mesg = $.growl.warning({ message: "The PPT generation failed. There was an error and it has been intimated to the DARA Team. We will get back to you shortly with the report." });
                                        $('.loading').hide();
                                        alert("Rajesh");
                                    }
                                },
                                error: function (request, status, error, e) {
                                    mesg = $.growl.warning({ message: "The PPT generation failed. There was an error and it has been intimated to the DARA Team. We will get back to you shortly with the report." });
                                    $('.loading').hide();
                                    //alert(e.message);
                                }
                            });
                        }
                        else {
                            mesg = $.growl.error({ message: "Please select a valid End Date!" });
                        }
                    }
                    else {
                        mesg = $.growl.error({ message: "Please select a valid Startdate!" });
                    }
                }
                else {
                    mesg = $.growl.error({ message: "Please select a valid Facility!" });
                }
            }

            else {
                mesg = $.growl.error({ message: "Please select a valid Member!" });
            }

            return false;
        });

        $('#Reset').click(function (e) {
            location.reload();
        });

    });
