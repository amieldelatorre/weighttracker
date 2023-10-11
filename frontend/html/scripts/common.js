function clearNotification() {
  let notificationModal = document.getElementById('notification-modal');
  
  if (notificationModal != null) {
    notificationModal.remove();
  }
}

function addGenericNotification(notification) {
  let wrapper = document.getElementById("wrapper");

  let notificationModal = document.createElement("div");
  notificationModal.id = "notification-modal";
  notificationModal.classList.add("modal");

  if (notification.type === "error") {
    notificationModal.classList.add("error-modal")
  } else if (notification.type === "success") {
    notificationModal.classList.add("success-modal")
  }

  let notificationModalExit = document.createElement("span");
  notificationModalExit.classList = "modal-exit";
  notificationModalExit.onclick = clearNotification;
  notificationModalExit.innerHTML = "&times;";
  notificationModal.appendChild(notificationModalExit);

  let notificationModalTitle = document.createElement("h2");
  notificationModalTitle.id = "modal-heading";
  notificationModalTitle.innerText = notification.title;
  notificationModal.appendChild(notificationModalTitle);

  notification.messages.forEach((message) => {
    let messageElement = document.createElement("p");
    messageElement.innerText = message;
    notificationModal.append(messageElement);
  });

  wrapper.insertBefore(notificationModal, wrapper.firstChild);
}