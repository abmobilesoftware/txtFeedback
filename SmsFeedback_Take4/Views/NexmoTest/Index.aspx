<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SmsFeedback_Take4.Models.WorkingPoint>>" %>
<%@ Import Namespace="SmsFeedback_Take4.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
   <style>
      #txtBody {
         float: left;
         width: 300px;
         height:  100px;
      }
      #btnSendMsg {
         float: left;
         width:308px;
         line-height:33px;       
      }
      .fromToBox {
         float:left;     
      }
      .label {
         width: 40px;
         min-width: 40px;
         display: inline-block;
      }
      #container {        
         top: 200px;
         -webkit-border-radius: 5px;
         -moz-border-radius: 5px;
         border-radius: 5px;
         padding:5px;         
         position:relative;
         display:block;
         margin-left: auto;
         margin-right: auto;
         width: 700px;
         min-width: 700px;
      }      
      table#details {
         width: 300px;
         min-width: 300px;
         max-width: 300px;
         float:right;         
	      border-width: 1px;
	      border-spacing: 0px;
	      border-style: solid;
	      border-color: gray;
	      border-collapse: separate;
	      background-color: white;
      }
      table#details th {
	      border-width: 1px;
	      padding: 3px;
	      border-style: solid;
	      border-color: gray;
	      background-color: white;
	      -moz-border-radius: ;
      }
      table#details td {
	      border-width: 1px;
	      padding: 3px;
	      border-style: solid;
	      border-color: gray;
	      background-color: white;
	      -moz-border-radius: ;
      }
      #styler {
         display: inline-block;         
         background-color: #DDD;
         padding: 5px;
         -webkit-border-radius: 3px;
         -moz-border-radius: 3px;
         border-radius: 3px;
      }
      .inputNr {
         width:258px;
      }
   </style>
    <title>New Index</title>
</head>
<body>
   <script src="../../Scripts/jquery-1.6.2.min.js" charset="charset="UTF-8"></script>
   <script src="../../Nexmo/nexmoTest.js"></script>
   <div id="container">
      <div id="styler">
       <table id="details">
            <tr>
               <th>Working point name</th>
               <th>Telephone number</th>
            </tr>
            <% foreach (WorkingPoint wp in Model)
               { %>
		 <tr>
          <td><%= wp.Name %></td>
          <td> <%= wp.TelNumber %></td>
		 </tr>
	         <%} %>
         </table>      
      <textarea id="txtBody"></textarea>      
      <div class="fromToBox">
         <span class="label">From:</span>
         <input type="text" id="fromNumber" class="inputNr" />
      </div>
      <div class="fromToBox">
         <span class="label">To:</span>
         <input type="text" id="toNumber" class="inputNr"/>
         </div>      
         <button id="btnSendMsg">Send Message with Nexmo</button>
      
        </div>
      </div>
</body>
</html>
