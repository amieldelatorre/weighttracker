function currentLocationIsLoginPage() {
  const loginPageRegex = new RegExp("\/?login(?:\.html)?");
  const pathName = window.location.pathname;

  return loginPageRegex.test(pathName);
}

function clearCredentialsAndRedirectToLogin() {
  localStorage.removeItem("email");
  localStorage.removeItem("password");
  
  console.log(!currentLocationIsLoginPage())

  if (!currentLocationIsLoginPage()) {
    window.location.replace("/login");
  }
}

async function isLoggedIn() {
  let email = localStorage.getItem("email");
  let password = localStorage.getItem("password");
  if (email === null || password === null) {
    return false;
  } else {
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
    }).then(response => {
      if (response.ok) { return true; } 
      else {
        return false;
      }
    }).catch(error => {
      return false;
    });
  }
}

function clearNotification() {
  let notificationModal = document.getElementById('notification-modal');
  
  if (notificationModal != null) {
    notificationModal.remove();
  }
}

function createNotificationModal(notificationType) {
  let notificationModal = document.createElement("div");
  notificationModal.id = "notification-modal";
  notificationModal.classList.add("modal");

  if (notificationType === "error") {
    notificationModal.classList.add("error-modal")
  } else if (notificationType === "success") {
    notificationModal.classList.add("success-modal")
  }
  
  return notificationModal;
}

function createNotificationModalExit() {
  let notificationModalExit = document.createElement("span");
  notificationModalExit.classList = "modal-exit";
  notificationModalExit.onclick = clearNotification;
  notificationModalExit.innerHTML = "&times;";

  return notificationModalExit;
}

function createNotificationModalTitle(notificationTitle) {
  let notificationModalTitle = document.createElement("h2");
  notificationModalTitle.id = "modal-heading";
  notificationModalTitle.innerText = notificationTitle;
  return notificationModalTitle;
}

function addGenericNotification(notification) {
  let wrapper = document.getElementById("wrapper");
  let notificationModal = createNotificationModal(notification.type);
  
  notificationModal.appendChild(createNotificationModalExit());
  notificationModal.appendChild(createNotificationModalTitle(notification.title));

  notification.messages.forEach((message) => {
    let messageElement = document.createElement("p");
    messageElement.innerText = message;
    notificationModal.append(messageElement);
  });

  wrapper.insertBefore(notificationModal, wrapper.firstChild);
}

function setValidationErrorModal(errorJSONPromise) {
  let wrapper = document.getElementById("wrapper");
  let notificationModal = createNotificationModal("error");

  notificationModal.appendChild(createNotificationModalExit());
  notificationModal.appendChild(createNotificationModalTitle("Input Errors!"));

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

function getAuthHeaderValue() {
  let email = localStorage.getItem("email");
  let password = localStorage.getItem("password");
  let encodedBasicAuth = btoa(`${email}:${password}`);

  return `Basic ${encodedBasicAuth}`;
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}