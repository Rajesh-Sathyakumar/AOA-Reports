<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="DAReportsAutomation.Home" %>
<!DOCTYPE html>

<html>

<!-- Head/CSS Scripts -->
<head>
    <title>Automated Opportunity Analysis</title>
            <link href="Luna/css/lunalite.min.css" rel="stylesheet" />
            <script src="Luna/js/modernizr.min.js"></script>
            <link href="crux/css/widgets.min.css" rel="stylesheet" />
            <link href="Content/Loading.css" rel="stylesheet" />
            <link href="jquery-growl-master/stylesheets/jquery.growl.css" rel="stylesheet" />-->
            <!--<link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/dropdown.css" rel="stylesheet" />
        <link href="Content/bootstrap-multiselect.css" rel="stylesheet" />-->

            <script src="Scripts/jquery-1.9.1.js"></script>
        <!--<script src="Scripts/bootstrap.js"></script>-->
        <!---->
        <script src="Luna/js/lunalite.min.js"></script>
        <script src="Luna/js/crux.min.js"></script>
        <script src="Scripts/handlebars-v1.3.0.js"></script>
        <script src="crux/js/widgets/cs.popover.js"></script>
        <script src="jquery-growl-master/javascripts/jquery.growl.js"></script>
        <!--<script src="Scripts/bootstrap-multiselect.js"></script>-->             
        <script src="Scripts/Home.js"></script>
            

</head>


<body>
    <header id="header">
        <div class="container title">
            <div class="row">
                <div class="col6">
                    <h1>
                        <a href="/">
                            <i id="abclogo">
                                <canvas></canvas>
                            </i>
                            <em>CRIMSON<small>Automated Opportunity Analysis</small></em>
                        </a>
                    </h1>
                </div>
               
            </div>
        </div>
        <nav id="nav">
            <div class="container">
                <div class="row">
                    <div class="col12">
                        <ul class="primary">
                            <li id="nav-PDA" class="no-menu current"><a href="#">Key Readmission Findings by Condition </a></li>
                           
                        </ul>
                    </div>
                </div>
            </div>
        </nav>
    </header>

    <div id="body">
        <form id="DARA" runat="server">
            <div class="container">
                <h1 id="page_title" data-nav="Components">Welcome To AOA</h1>
                <h4 id="page-sub-title" style="text-align:left">Key Readmission Findings by Condition
                </h4>
                    
               
                <br />

                <div class="loading" id="LoadingScreen" runat="server" style="display: none;">
                    <div class="spinner" id="spin" runat="server"></div>
                </div>

                <div class="alert-info">
                    <span id="mesg"></span>
                </div>

                <div class="row">
                    
                    <div class="col6">
                        <strong>Data Loaded Till: </strong>
                      <div id="maxdischarge"><select>
                                <option class="disabled">mm-dd-yyyy</option>
                       </select>
                          </div>
                    </div>

      

                </div>
				
				<div class="row">
                    
                    <div class="col6">
                        <strong>Member Database</strong>
                        <div id="DBDropDown">
                            <select>
                                <option class="disabled">--Select--</option>
                            </select>
                        </div>
                    </div>

                    <div class="col6">
                        <strong>Facilities :</strong>
                        <div id="HosDropDown">
                            <select id="example-enableFiltering-includeSelectAllOption" multiple="multiple">
                                <%--<option class="disabled">Option 1</option>
                                <option class="disabled">Option 1</option>--%>

                            </select>
                           
                             
                            <%--<select id="example-enableFiltering-includeSelectAllOption" multiple="multiple" data-ng-controller="ctrl1">
                                            <option value="1">{{hospital[$index].HospitalName}}</option>
                                          
                                        </select>--%>                                                       
                        </div>
                    </div>

                </div>

                <div class="row">
                   
                    <div class="col6">
                        <strong>Start Date:</strong>
                        <div id="Startdate">
                            <input id="startdt" type="date" name="bday" min="2000-01-01" max="2020-12-31"/>
                        </div>
                    </div>

                    <div class="col6">
                        <strong>End Date:</strong>
                        <div id="EndDate">
                            <input id="enddt" type="date" name="bday" min="2000-01-01" max="2020-12-31"/>
                        </div>
                    </div>
                </div>
                
                <br />

                <br />

                <button type="submit" id="Submit" class="btn primary" style="margin: 0 auto;">Submit</button>
                <button type="button" id="Reset" class="btn inverse" style="margin: 0 auto">Reset</button>

                <div id="growls" class="modal fade in">
                    <div class="growl growl-error growl-medium growl-outgoing">
                        <div class="growl-close"></div>
                        <div class="growl-title"></div>
                        <div class="growl-message"></div>
                    </div>
                </div>
            </div>
      </form>
    </div>
    <br />

    <footer id="footer">
        <div class="container">
            <div id="copyright">Copyright © 2005-2014, <a href="http://advisory.com/" target="_blank">The Advisory Board Company</a></div>
        </div>
    </footer>
    <!-- Handlebar Template -->
<script type="text/x-handlebars-template" id="dbTemplate">
    <select class="selectr">
        {{#each rows}}
    <option value="{{ReleaseID}}" data-value="{{Name}}">{{Name}}</option>
        {{/each}}
    </select>
</script>
    
    <script type="text/x-handlebars-template" id="maxdischargeTemplate">
    <select class="selectr">
        {{#each rows}}
    <option  data-value="{{MaxDischargeDate}}">{{MaxDischargeDate}}</option>
        {{/each}}
    </select>
</script>
<script type="text/x-handlebars-template" id="hosTemplate">
                {{#each rows}}
                    <option  value="{{HospitalName}}">{{HospitalName}}</option>
                {{/each}}
</script>
<script type="text/x-handlebars-template" id="test-template">
                {{#each rows}}
                    <option  value="{{HospitalName}}">{{HospitalName}}</option>
                {{/each}}
</script>
</body>
<%--<script type="text/x-handlebars-template" id="hosTemplate123123">
    <select class="selectr">
        {{#each rows}}
    <option  data-value="{{HospitalName}}">{{HospitalName}}</option>
        {{/each}}
    </select>
</script>--%>

</html>
