<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
         clear: both;
      }
      
   </style>
    <title>Index</title>
</head>
<body>
   <script src="../../Scripts/jquery-1.6.2.min.js"></script>
   <script src="../../Nexmo/nexmoTest.js"></script>
    <div>
       <textarea id="txtBody" ></textarea> 
       <button id="btnSendMsg">Send Message with Nexmo</button>
    </div>
</body>
</html>
