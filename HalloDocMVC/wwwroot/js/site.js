/* Password */

function passtoggle() {
    var x = document.getElementById("Password");
    if (x.type == "password") {
        x.type = "text";
        document.querySelectorAll("i.fa.fa-eye-slash").forEach(i => i.style.display = "none");
        document.querySelectorAll("i.fa.fa-eye").forEach(i => i.style.display = "block");
    }
    else {
        x.type = "password";
        document.querySelectorAll("i.fa.fa-eye-slash").forEach(i => i.style.display = "block");
        document.querySelectorAll("i.fa.fa-eye").forEach(i => i.style.display = "none");
    }
}