async function handleLoginFormSubmit() {
  let loginForm = document.getElementById("login-form");
  loginForm.addEventListener("submit", async (submitEvent) => {
    submitEvent.preventDefault();

    let email = document.getElementById("email").value;
    let password = document.getElementById("password").value;

    let data = {
      "email": email,
      "password": password
    }

    await fetch(url=`${API_URL}/Auth/login`, {
      method: "POST",
      cors: "no-cors",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(data)
    })
    .then(response => {
      if (response.ok) {
        localStorage.setItem("email", email);
        localStorage.setItem("password", password);

        window.location.replace(`/index.html`);
      } else {
        if (response.status === 401) {
          clearNotification();
          
          let notification = {
            type: "error",
            title: "Email and Password combination was not recognized.",
            messages: [
              "Please try again."
            ]
          };
          addGenericNotification(notification);
        } else {
          clearNotification();

          let notification = {
            type: "error",
            title: "Unable to process request 😞",
            messages: [
              "Please try again later and if problem persists, please contact the administrator."
            ]
          };
          addGenericNotification(notification);
        }
      }
    }).catch(error => {
      clearNotification();

      let notification = {
        type: "error",
        title: "Unable to reach server 😞",
        messages: [
          "Please try again later and if problem persists, please contact the administrator."
        ]
      };
      addGenericNotification(notification);
    });
  });
}

// Do this straight away
isLoggedIn().then(
  response => {
    if (response) {
      window.location.replace("/");
    }
  }
);

window.addEventListener("load", () => {
  handleLoginFormSubmit();

  let params = (new URL(document.location)).searchParams;
  let signupSuccess = params.get("signup");

  if (signupSuccess != null && signupSuccess.trim() != "") {
    clearNotification();

    let notification = {
      type: "success",
      title: "Signup successful 🎉",
      messages: [
        "Please login ☺️"
      ]
    };

    addGenericNotification(notification);
  } 
});
