function validatePasswordMatch() {
  let password = document.getElementById("password");
  let confirmPassword = document.getElementById("confirmPassword");

  if (password.value != confirmPassword.value) {
    confirmPassword.setCustomValidity("Passwords do not match!");
  } 
  else {
    confirmPassword.setCustomValidity('');
  }
}


window.addEventListener("load", () => {
  let loginForm = document.getElementById("signup-form");
  loginForm.addEventListener("submit", (e) => {
    e.preventDefault();

    let firstName = document.getElementById("firstName").value;
    let lastName = document.getElementById("lastName").value;
    let email = document.getElementById("email").value;
    let password = document.getElementById("password").value;
    let dateOfBirth = document.getElementById("dateOfBirth").value;
    let height = document.getElementById("height").value;
    let gender = document.getElementById("gender").value;

    let data = {
      "firstName": firstName,
      "lastName": lastName,
      "email": email,
      "password": password,
      "dateOfBirth": dateOfBirth,
      "gender": gender,
      "height": height
    }

    fetch(URL=`${API_URL}/User`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(data)
    })
    .then(res => res.json())
    .then(data => console.log(data))
    .catch(err => console.log(err));
  });
});

