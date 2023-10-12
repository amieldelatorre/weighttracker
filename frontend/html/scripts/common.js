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