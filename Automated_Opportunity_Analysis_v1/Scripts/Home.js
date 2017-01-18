var MaxDischargeDate;

$(function () {
    var serverURL = "";

    $(document).ready(function () {
        $('#MemberDropDown').spinner;

        $('.dately').dately();

   

        $.ajax({
            type: "GET",
            url: serverURL + "Home.aspx/GetUsername",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                document.getElementById("username").innerHTML = JSON.parse(response.d);
            }
        });

        $.ajax({
            type: "GET",
            url: serverURL + "Home.aspx/GetMemberList",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                var source = document.getElementById("MemberTemplate").innerHTML;
                var template = Handlebars.compile(source);
                document.getElementById("MemberDropDown").innerHTML = template({ rows: JSON.parse(response.d) });

                $('#MemberDropDown').trigger('change');
            }
        });
            
    });

    $('#MemberDropDown').change(function () {

        var MemberName = $(this).find("option:selected").text();

        if (MemberName == "None Selected") {


            if ($('#example-enableFiltering-includeSelectAllOption').data('crux-selectr')) {
                $('#example-enableFiltering-includeSelectAllOption').selectr('destroy');
                $('#example-enableFiltering-includeSelectAllOption').show();
            }
            $('#example-enableFiltering-includeSelectAllOption').html("");
            $('#example-enableFiltering-includeSelectAllOption').selectr({
                width: 350,
                listwidth: 350
            });

            document.getElementById("maxdischarge").innerHTML = "None";

        }
        else {


            MemberName = MemberName.replace(/'/g, "\\'");
            $.ajax({
                type: "GET",
                url: serverURL + "Home.aspx/GetHosList",
                contentType: "application/json; charset=utf-8",
                data: { ProjectName: "'" + MemberName + "'" },
                dataType: "json",
                success: function (data) {

                    var rows = JSON.parse(data.d);
                    var optionEl = "";
                    for (var i = 0; i < rows.length; i++) {
                        optionEl += "<option  value='" + rows[i].HospitalKey + "'>" + rows[i].HospitalName + "</option>";
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


                    $.ajax({
                        type: "GET",
                        url: serverURL + "Home.aspx/GetMaxDischargeDate",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {

                            var source = document.getElementById("maxdischargeTemplate").innerHTML;
                            var template = Handlebars.compile(source);

                            MaxDischargeJSON = JSON.parse(data.d);
                            MaxDischargeDate = Date.parse(MaxDischargeJSON[0].MaxDischargeDate);

                            document.getElementById("maxdischarge").innerHTML = template({ rows: MaxDischargeJSON });

                        },

                        error: function (data, success, error) {
                            alert(error);
                        }
                    });

                },

                error: function (data, success, error) {
                    alert(error);
                }
            });


        }

    });
        

        $('#Submit').click(function () {
            
            var MemberName = $("#MemberDropDown").find("option:selected").text();

            var hospitalList = "";
            //var selectedHospitals = $($('#example-enableFiltering-includeSelectAllOption').selectr('html', 'list')).find(':checked');

            //selectedHospitals.each(function (i, el) {
            //    hospitalList += $(el).parent().find('label').text();
            //    hospitalList += (i < (selectedHospitals.length - 1)) ? ',' : '';
            //});


            myList = [];
            $('#example-enableFiltering-includeSelectAllOption').each(function () {
                myList.push($(this).val())
            });
            
            var i;

            for (i = 0; i < myList.length; i++) {
                    hospitalList += myList[i];
                    hospitalList += (i < (myList.length - 1)) ? ',' : '';
            }
                
            var Startdate = Date.parse($('#startdt').val());
            var Enddate = Date.parse($('#enddt').val());


            if (MemberName != "None Selected") {

                
                if (hospitalList && hospitalList != "") {

                    if ( $('#startdt').val() != "") {

                        if ( $('#enddt').val() != "") {
                            if (Startdate <= Enddate) {

                                if (Startdate <= MaxDischargeDate) {

                                    if (Enddate > MaxDischargeDate) {
                                        mesg = $.growl.warning({
                                            message: "Please Note that the Generated Report will contain results only up till the 'Data Loaded Till Date' !"
                                            ,delayOnHover: true,
                                            duration: 10000
                                        });
                                    }

                                    $('.loading').show();

                                    $.ajax({
                                        type: "GET",
                                        url: serverURL + 'Home.aspx/Register',
                                        data: { hospital: "'" + hospitalList + "'", startdate: "'" + $('#startdt').val() + "'", enddate: "'" + $('#enddt').val() + "'" },
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (data) {
                                            if (data.d == 'Success') {
                                                //mesg.style.color = "green";
                                                mesg = $.growl.notice({
                                                    message: "Your report has been successfully generated. You will recieve an email shortly."
                                                    , delayOnHover: true,
                                                    duration: 10000
                                                });
                                                $('.loading').hide();
                                                e.preventDefault();
                                            }
                                            else if (data.d == 'Failure') {
                                                mesg = $.growl.warning({
                                                    message: "The PPT generation failed. There was an error and it has been intimated to the DARA Team. We will get back to you shortly with the report."
                                                , delayOnHover: true,
                                                    duration: 10000
                                                });
                                                $('.loading').hide();
                                                alert("Rajesh");
                                            }
                                        },
                                        error: function (request, status, error, e) {
                                            mesg = $.growl.warning({
                                                message: "The PPT generation failed. There was an error and it has been intimated to the DARA Team. We will get back to you shortly with the report."
                                                , delayOnHover: true,
                                                duration: 10000
                                            });
                                            $('.loading').hide();
                                            //alert(e.message);
                                        }
                                    });
                                }
                                else {
                                    mesg = $.growl.error({
                                        message: "Error: Please Choose an StartDate that is lesser than the 'Data Loaded Till' Date !"
                                        , delayOnHover: true,
                                        duration: 10000
                                    });
                                }
                                
                            }
                            else {
                                mesg = $.growl.error({
                                    message: "Error: Please Choose an EndDate that is greater than Startdate!"
                                    , delayOnHover: true,
                                    duration: 10000
                                });
                            }
                        }
                        else {
                            mesg = $.growl.error({
                                message: "Error: Please select a valid End Date!"
                                , delayOnHover: true,
                                duration: 10000
                            });
                        }
                    }
                    else {
                        mesg = $.growl.error({
                            message: "Error: Please select a valid Startdate!"
                            , delayOnHover: true,
                            duration: 10000
                        });
                    }
                }
                else {
                    mesg = $.growl.error({
                        message: "Error: Please select a valid Facility!"
                        , delayOnHover: true,
                        duration: 10000
                    });
                }
            }

            else {
                mesg = $.growl.error({
                    message: "Error: You have not selected a Member. Please select a Member Name for Report Generation !"
                    , delayOnHover: true,
                    duration: 10000
                });
            }

            return false;
        });

        $('#Reset').click(function (e) {
            window.location.replace("http://atxcccwapp-s02/Home.aspx");
        });

    });
