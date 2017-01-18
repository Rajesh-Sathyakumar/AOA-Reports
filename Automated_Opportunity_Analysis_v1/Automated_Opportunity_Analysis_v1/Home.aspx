<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="DAReportsAutomation.Home" %>
<!DOCTYPE html>

<html>

<head>
            <title>CCC | AOA Reports</title>
            <link rel="shortcut icon" href="favicon.ico" />
            <link href="Luna/css/lunalite.min.css" rel="stylesheet" />
            <script src="Luna/js/modernizr.min.js"></script>
            <link href="crux/css/widgets.min.css" rel="stylesheet" />
            <link href="Content/Loading.css" rel="stylesheet" />
            <link href="jquery-growl-master/stylesheets/jquery.growl.css" rel="stylesheet" />
    
            <script src="Scripts/jquery-1.9.1.js"></script>
            <script src="Luna/js/lunalite.min.js"></script>
            <script src="Luna/js/crux.min.js"></script>
            <script src="Scripts/handlebars-v1.3.0.js"></script>
            <script src="crux/js/widgets/cs.popover.js"></script>
            <script src="jquery-growl-master/javascripts/jquery.growl.js"></script>
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
               <div class="col6">
                    <ul class="utilities">
                            <li class="dropdown">
                                <div>
                                    <a>
                                        Hello, <b id="username" ></b>
                                        <i class="caret4 south"></i>
                                    </a>
                                    <ul>
                                        <li class="logout"><a href= "<%= Page.ResolveUrl("~\\Logout.aspx") %>" class="sign-out" id="sign-out">Sign Out</a></li>
                                    </ul>

                                  </div>

                            </li>
                    </ul>
                </div>
            </div>
        </div>
        <nav id="nav">
            <div class="container">
                <div class="row">
                    <div class="col12">
                        <ul class="primary">
                            <li id="nav-PDA" class="no-menu current"><a href="#">Readmission Analytics </a></li>
                           
                        </ul>
                    </div>
                </div>
            </div>
        </nav>
    </header>

    <div id="body">
        <form id="DARA" runat="server">
            <div class="container">
                <h1 id="page_title" data-nav="Components">Welcome To Readmission PPT OA</h1>
               
                <br />


                <div class="loading" id="LoadingScreen" runat="server" style="display: none;">
                    <div class="spinner" id="spin" runat="server"></div>
                </div>

                <div class="alert-info">
                    <span id="mesg"></span>
                </div>

                <div class="row">
                    
                    <div class="col6"> <p></p></div>

                    <div class="col2">
                        <strong>Data Loaded Till</strong>
                      <div id="maxdischarge">

                          </div>
                    </div>

                </div>

                <div class="row"> <p></p></div>
				
				<div class="row">
                    
                    <div class="col6">
                        <strong>Member Name</strong>
                        <div id="MemberDropDown">
                            
                        </div>
                    </div>

                    <div class="col6">
                        <strong>Facilities</strong>
                        <div id="HosDropDown">
                            <select id="example-enableFiltering-includeSelectAllOption" multiple="multiple">

                            </select>
                                                  
                        </div>
                    </div>

                </div>

                <div class="row"><p></p></div>

                <div class="row">
                   
                    <div class="col6">
                        <strong>Start Date</strong>
                        <div id="Startdate">
                            <input id="startdt" class="dately" name="bday" data-cs-mindate="01/01/2000" data-cs-maxdate="01/01/2020"/>
                        </div>
                    </div>

                    <div class="col6">
                        <strong>End Date</strong>
                        <div id="EndDate">
                            <input id="enddt" class="dately" name="bday" data-cs-mindate="01/01/2000" data-cs-maxdate="01/01/2020"/>
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
            <div id="copyright">Copyright © 2016, <a href="http://advisory.com/" target="_blank">The Advisory Board Company</a></div>
        </div>
    </footer>
    <!-- Handlebar Template -->
<script type="text/x-handlebars-template" id="MemberTemplate">
    <select class="selectr" id="memberselect">
        <option value="None Selected" data-value="None Selected" selected="selected">None Selected</option>
        {{#each rows}}
    <option value="{{ReleaseID}}" data-value="{{ProjectName}}">{{ProjectName}}</option>
        {{/each}}
    </select>
</script>
    
    <script type="text/x-handlebars-template" id="maxdischargeTemplate">
    <span class="message">
        {{#each rows}}
            {{MaxDischargeDate}}
        {{/each}}
    </span>
       
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

</html>
