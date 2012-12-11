function DownloadJSON2CSV(objArray, title) {
   var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
   var cols = array.cols;
   var rows = array.rows;

   // cols 2 csv
   var str = '';
   var line = '';
   for (var k = 0; k < cols.length; k++) {

      line += cols[k].label + ',';

      // Here is an example where you would wrap the values in double quotes
      // for (var index in array[i]) {
      //    line += '"' + array[i][index] + '",';
      // }

      line.slice(0, line.Length - 1);
   }
   str += line + '\r\n';

   // rows 2 csv
   for (var i = 0; i < rows.length; i++) {
      line = '';
      for (var j = 0; j < rows[i].c.length; j++) {
         line += rows[i].c[j].v + ',';
      }

      line.slice(0, line.length - 1);
      str += line + '\r\n';
   }

   //window.open("data:text/csv;charset=utf-8," + escape(str))
   $("#reportContent").append('<form id="exportform" action="http://46.137.26.124/export2csv.php" method="post" target="_blank">' +
      '<input type="hidden" id="exportdata" name="exportdata" />' + 
      '<input type="hidden" id="docTitle" name="docTitle" />' +
   '</form>');
   $("#exportdata").val(str);
   $("#docTitle").val(title.replace(/ /g, "_"));
   $("#exportform").submit().remove();
}