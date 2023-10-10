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
  const errorModal = document.getElementById("signup-form-error-modal");
  let errorModalHeading = document.createElement("h2");
  errorModalHeading.id = "error-modal-heading";
  errorModalHeading.innerText = "Input Errors!";

  errorModal.appendChild(errorModalHeading);


  errorJSONPromise.then(errorJSON => {
  
    for (const [errorFieldName, fieldErrorList] of Object.entries(errorJSON)) {
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
  
      errorModal.appendChild(newErrorSection); 
    }
  
  });
  errorModal.style.display = "block";
}

function setUnableToProcessRequestModal() {
  const errorModal = document.getElementById("signup-form-error-modal");

  let errorModalHeading = document.createElement("h2");
  errorModalHeading.id = "error-modal-heading";
  errorModalHeading.innerText = "Unable to process request 😞";
  errorModal.appendChild(errorModalHeading);

  let errorModalText = document.createElement("p");
  errorModalText.innerText = "Please try again later and if problem persists, please contact the administrator.";
  errorModal.appendChild(errorModalText);

  errorModal.style.display = "block";
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
        window.location.replace(`/login?signup=success`)
      }
      else {
        if (response.status === 422) {
          let errorJSONPromise = response.json().then(err => err["errors"]);
          setValidationErrorModal(errorJSONPromise);
        } else {
          setUnableToProcessRequestModal();
        }
      }
    })
    .catch(error => {
      alert(error)
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
//       if (res.status === 422) {
//         let errorJSONPromise = res.json().then(err => err["errors"]);
//         setValidationErrorModal(errorJSONPromise);
//       } else {
//         setUnableToProcessRequestModal();
//       }
//     }
//   })
//   .catch(err => {
//     alert(err)
//   });
// });
