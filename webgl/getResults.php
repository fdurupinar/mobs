<?php

   // These settings define where the server is located, and
    // which credentials we use to connect to that server.  
    
    $server   = "umbvr.cs.umb.edu";
    $username = "root";
    $password = "Motion12!";
    
    // This is the database and table we are going to access in
    // the mysql server. In this example, we assume that we have
    // the table 'highscores' in the database 'gamedb'.
    
    $database = "mobsDocileDB";
    $table    = "docileAgentValid";
    
    
    $connection = mysqli_connect($server, $username, $password, $database);
    

    
    
   
// fetch the data

$rows = mysql_query('SELECT * FROM ' . $table);
$rows || die(mysql_error());


// create a file pointer connected to the output stream
$output = fopen('output.csv', 'w');

// output the column headings

$fields = [];
for($i = 0; $i < mysql_num_fields($rows); $i++) {
    $field_info = mysql_fetch_field($rows, $i);
    $fields[] = $field_info->name;
}
fputcsv($output, $fields);

// loop over the rows, outputting them
while ($row = mysql_fetch_assoc($rows)) fputcsv($output, $row);
	  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>