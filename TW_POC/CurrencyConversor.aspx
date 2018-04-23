<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CurrencyConversor.aspx.cs" Inherits="TW_POC.CurrencyConversor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
        <div>
            <input type="file" id="File1" name="File1" runat="server" />
            <br />
            <input type="submit" id="Submit1" onclick="Submit1_ServerClick()" value="Upload input data" runat="server" />
        </div>
    </form>
</body>
</html>
