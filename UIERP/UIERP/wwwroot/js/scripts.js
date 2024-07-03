window.toggleElement = (elementId) => {
    var element = document.getElementById(elementId);
    if (element) {
        element.style.display = element.style.display === "none" ? "block" : "none";
    }
};