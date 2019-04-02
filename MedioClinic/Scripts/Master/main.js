document.addEventListener("DOMContentLoaded", function () {
    var sidenavElement = document.querySelector(".sidenav");
    M.Sidenav.init(sidenavElement);
    var dropdownElements = document.querySelectorAll(".dropdown-trigger");

    M.Dropdown.init(dropdownElements, {
        hover: false
    });
});