<?php

function printJson($obj)
{
    header("Content-Type: application/json");
    print json_encode($obj, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
}