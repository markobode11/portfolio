<?php


class Author
{
    public $id;
    public $firstName;
    public $lastName;
    public $grade;

    function __construct($id, $firstName, $lastName, $grade)
    {
        $this->id = $id;
        $this->firstName = $firstName;
        $this->lastName = $lastName;
        $this->grade = $grade;
    }

    function getName()
    {
        return $this->firstName . " " . $this->lastName;
    }

}