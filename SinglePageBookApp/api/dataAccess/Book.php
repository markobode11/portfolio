<?php


class Book
{
    public $id;
    public $title;
    public $isRead;
    public $grade;
    public $authors = [];

    function __construct($title, $isRead, $grade, $id = null)
    {
        $this->id = $id;
        $this->title = $title;
        $this->isRead = $isRead;
        $this->grade = $grade;
    }

    function addAuthor($author)
    {
        $this->authors[] = $author;
    }

    public static function fromAssocArray($array) {
        return new Book($array['title'], $array['isRead'], $array['grade']);
    }

}