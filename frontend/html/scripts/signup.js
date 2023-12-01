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

function handleSignupFormSubmit() {
  let signupForm = document.getElementById("signup-form");
  signupForm.addEventListener("submit", async (submitEvent) => {
    submitEvent.preventDefault();

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

    await fetch(URL=`${API_URL}/User`, {
      method: "POST",
      cors: "no-cors",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(data)
    })
    .then(response => {
      if (response.ok) {
        window.location.replace(`/login?signup=success`);
      }
      else {
        if (response.status === 400) {
          clearNotification();
          let errorJSONPromise = response.json().then(err => err["errors"]);
          setValidationErrorModal(errorJSONPromise);
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
    })
    .catch(error => {
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

window.addEventListener("load", () => {
  handleSignupFormSubmit();
});

// window.addEventListener("load", async () => {
//   let data = {
//     "firstName": "firstName",
//     "lastName": "lastName",
//     "email": "james.smith@example.com",
//     "password": "password",
//     "dateOfBirth": "2023-10-10",
//     "gender": "MALE",
//     "height": 1
//   }

//   await fetch(URL=`${API_URL}/User`, {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json"
//     },
//     body: JSON.stringify(data)
//   })
//   .then(res => {
//     if (res.ok) {
//       window.location.replace(`/login?signup=success`)
//     }
//     else {
//       if (res.status === 400) {
//         let errorJSONPromise = res.json().then(err => err["errors"]);
//         clearNotification();
//         setValidationErrorModal(errorJSONPromise);
        
//       } else {
//         clearNotification();

//         let notification = {
//           type: "error",
//           title: "Unable to process request 😞",
//           messages: [
//             "Please try again later and if problem persists, please contact the administrator."
//           ]
//         };
//         addGenericNotification(notification);
//       }
//     }
//   })
//   .catch(err => {
//     clearNotification();

//     let notification = {
//       type: "error",
//       title: "Unable to reach server 😞",
//       messages: [
//         "Please try again later and if problem persists, please contact the administrator."
//       ]
//     };
//     addGenericNotification(notification);
//   });
// });
