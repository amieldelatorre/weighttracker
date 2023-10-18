function handleSignOut() {
  let signOutButton = document.getElementById("signout-button");
  signOutButton.onclick = () => {
    clearCredentialsAndRedirect();
  };
}

window.addEventListener("load", async () => {
  handleSignOut();
});

// Do this straight away
checkLoggedIn();