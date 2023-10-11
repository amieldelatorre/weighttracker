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

function setValidationErrorModal(errorJSONPromise) {
  let wrapper = document.getElementById("wrapper");

  let notificationModal = document.createElement("div");
  notificationModal.id = "notification-modal";
  notificationModal.classList.add("modal");
  notificationModal.classList.add("error-modal");

  let notificationModalExit = document.createElement("span");
  notificationModalExit.classList = "modal-exit";
  notificationModalExit.onclick = clearNotification;
  notificationModalExit.innerHTML = "&times;";
  notificationModal.appendChild(notificationModalExit);

  let errorModalHeading = document.createElement("h2");
  errorModalHeading.id = "modal-heading";
  errorModalHeading.innerText = "Input Errors!";
  notificationModal.appendChild(errorModalHeading);

  errorJSONPromise.then(errorJSON => {
    for (const[errorFieldName, fieldErrorList] of Object.entries(errorJSON)) {
      let newErrorSection = document.createElement("section");
      let newErrorSectionLabel = document.createElement("p");
      newErrorSectionLabel.innerText = errorFieldName;
      newErrorSection.appendChild(newErrorSectionLabel);

      let newErrorSectionList = document.createElement("ul");
      newErrorSection.appendChild(newErrorSectionList);
      fieldErrorList.forEach((elem) => {
        let newErrorItem = document.createElement("li")
        newErrorItem.innerText = elem;
  
        newErrorSectionList.appendChild(newErrorItem);
      });
  
      notificationModal.appendChild(newErrorSection); 
    }
  });

  wrapper.insertBefore(notificationModal, wrapper.firstChild);
}

function handleSignupFormSubmit() {
  let loginForm = document.getElementById("signup-form");
  loginForm.addEventListener("submit", async (submitEvent) => {
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
