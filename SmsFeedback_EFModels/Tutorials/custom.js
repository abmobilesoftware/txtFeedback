jQuery(document).ready(function() {
    //$("a").click(function () {
    //    alert("Hello world!");
    //    event.preventDefault();       
    //});
    $("#orderedlist").addClass("red");
    $("#orderedlist > li").addClass("blue");    
    $("#orderedlist li:last").hover(function () {
        $(this).addClass("green");
    }, function () {
        $(this).removeClass("green");
    });
    $("#reset").click(function () {
        $("form").each(function () {
            this.reset();
        })
    });
    $("#faq").find('dd').hide().end().find('dt').click(function () {
        $(this).next().slideToggle();
    });
    $("a").hover(function () {
        $(this).parents("p").addClass("highlight");
    }, function () {
        $(this).parents("p").removeClass("highlight");
    });
    //$("li").not(":has(ul)").css("border", "1px solid black");
});