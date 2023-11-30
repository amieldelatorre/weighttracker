function handleSignOut() {
  let signOutButton = document.getElementById("signout-button");
  signOutButton.onclick = () => {
    clearCredentialsAndRedirect();
  };
}

function handleAddWeightFormSubmit() {
  let addWeightForm = document.getElementById("add-weight-form");
  addWeightForm.addEventListener("submit", async (submitEvent) => {
    submitEvent.preventDefault();
    let weightElement = document.getElementById("weight");
    let weight = weightElement.value;
    let dateOfWeightElement = document.getElementById("dateOfWeight");
    let dateOfWeight = dateOfWeightElement.value;
    let descriptionElement = document.getElementById("description");
    let description = descriptionElement.value;

    let data = {
      "userWeight": weight,
      "date": dateOfWeight,
      "description": description
    };


    await fetch(URL=`${API_URL}/Weight`, {
      method: "POST",
      cors: "no-cors",
      headers: {
        "Content-Type": "application/json",
        "Authorization": getAuthHeaderValue()
      },
      body: JSON.stringify(data)
    })
    .then(response => {
      if (response.ok) {
        clearNotification();
        let notification = {
          type: "success",
          title: "Weight measurement addition successful!",
          messages: [
            "Success !"
          ]
        };
        addGenericNotification(notification);
        weightElement.value = "";
        dateOfWeightElement.value = "";
        descriptionElement.value = "";
      } else {
        if (response.status === 400) {
          clearNotification();
          let errorJSONPromise = response.json().then(err => err["errors"]);
          setValidationErrorModal(errorJSONPromise);
        } else if (response.status === 401) {
          clearCredentialsAndRedirect();
        } else {
          clearNotification();

          let notification = {
            type: "error",
            title: "Unable to reach server 😞",
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

window.addEventListener("load", async () => {
  handleSignOut();
  handleAddWeightFormSubmit();
});

// Do this straight away
checkLoggedIn();