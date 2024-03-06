function handleSignOut() {
  let signOutButton = document.getElementById("signout-button");
  signOutButton.onclick = () => {
    clearCredentialsAndRedirectToLogin();
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


    await fetch(url=`${API_URL}/Weight`, {
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
          clearCredentialsAndRedirectToLogin();
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


async function getWeightData(limit=100, offset=0, dateFrom, dateTo) {
  let params = new URLSearchParams({
    limit: limit,
    offset: offset
  });
  if (dateFrom !== undefined ) {
    params.append("dateFrom", dateFrom)
  }
  if (dateTo !== undefined ) {
    params.append("dateTo", dateTo)
  }

  let weightURL = `${API_URL}/Weight?` + params;
  weightData = await fetch(url=weightURL, {
    method: "GET",
    cors: "no-cors",
    headers: {
      "Content-Type": "application/json",
      "Authorization": getAuthHeaderValue()
    }
  }).then(response => {
    if (response.ok) {
      return response.json()
    } else {
      if (response.status === 400) {
        clearNotification();

          let notification = {
            type: "error",
            title: "Unexpected issue with retrieving weight data 😞",
            messages: [
              "Please try again later and if problem persists, please contact the administrator."
            ]
          };
          addGenericNotification(notification);
      } else if (response.status === 401) {
        clearCredentialsAndRedirectToLogin();
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

  return weightData;
}


function createChart(weightDataResults) {
  new Chart(
    document.getElementById('chart').getContext('2d'),
    {
      type: 'line',
      data: {
        labels: weightDataResults.map(row => row.date),
        datasets: [
          {
            label: `Weight the past ${limit} days`,
            data: weightDataResults.map(row => row.userWeight),
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
          }
        ]
      }
    }
  );
}

function removeChildElements(parentElement) {
  while (parentElement.lastChild) {
    parentElement.removeChild(parentElement.lastChild)
  }
}

function getTableHeaderElement() {
  let tableHeaderElementRow = document.createElement("tr");
  tableHeaderElementRow.classList.add("weight-table-header-row");

  const headers = ["Date", "Weight", "Description", "Actions"];
  headers.forEach(header => {
    let childTableHeader = document.createElement("th");
    childTableHeader.innerText = header;
    tableHeaderElementRow.appendChild(childTableHeader);
  });

  return tableHeaderElementRow;
}

async function deleteWeight(weightId) {
  const deleteURL = `${API_URL}/Weight/${weightId}`;
  await fetch(url=deleteURL, {
    method: "DELETE",
    cors: "no-cors",
    headers: {
      "Content-Type": "application/json",
      "Authorization": getAuthHeaderValue()
    }
  }).then(async response => {
    if (response.ok) {
      clearNotification();
      let notification = {
        type: "success",
        title: "Weight deleted successfully!",
        messages: [
          "Success !",
          "Refreshing in 1.5s"
        ]
      };
      addGenericNotification(notification);
      await sleep(1500);
      window.location.reload();
    } else if (response.status === 404) {
      clearNotification();
      let notification = {
        type: "error",
        title: "The entry cannot be found and may have already been deleted",
        messages: [
          "Please refresh the page."
        ]
      };
      addGenericNotification(notification);
    } else if (response.status === 401) {
      clearCredentialsAndRedirectToLogin();
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
}

async function deleteWeightAction(weightId) {
  let weightData = (await data).results.filter(item => {
    return item.id == weightId;
  })[0]; // There should only be one weightId and it should already exist

  let weightDate = weightData.date;
  let deleteConfirmed = confirm(`Are you sure you want to delete the weight for the date ${weightDate}?`);

  if (deleteConfirmed) {
    await deleteWeight(weightId);
  }
}

function handleUpdateWeightFormSubmit() {
  let updateWeightForm = document.getElementById("update-weight-form");
  updateWeightForm.addEventListener("submit", async (submitEvent) => {
    submitEvent.preventDefault();
    let weightId = document.getElementById("update-weightId").value;
    console.log(weightId);
  });
}

function closeEditWeightForm() {
  let editFormModal = document.getElementById("edit-weight-form-modal");
  // Remove values and stuff
  if (editFormModal != null) {
    document.getElementById("update-weight-form").reset();
    document.getElementById("update-form-modify-time").innerText = "";
    document.getElementById("update-form-create-time").innerText = "";
    editFormModal.hidden = true;
  }
}

async function showEditForm(weightId) {
  console.log(weightId);
  let editFormModal = document.getElementById("edit-weight-form-modal");
  editFormModal.hidden = false;

  let weightData = (await data).results.filter(item => {
    return item.id == weightId;
  })[0]; // There should only be one weightId and it should already exist

  document.getElementById("update-weight-title").innerText = `Edit Entry for ${weightData.date}`;
  document.getElementById("description-update").value = weightData.description !== undefined ? weightData.description : "";
  document.getElementById("weight-update").value =  parseFloat(weightData.userWeight).toFixed(2);
  document.getElementById("dateOfWeight-update").value = weightData.date;
  document.getElementById("update-form-modify-time").innerText = weightData.dateModified;
  document.getElementById("update-form-create-time").innerText = weightData.dateCreated;
  document.getElementById("update-weightId").value = weightData.id;
}

function getRowActions(weightId) {
  let actionData = document.createElement("td");
  
  let editAction = document.createElement("img");
  editAction.src = "images/edit-svgrepo-com.svg"
  editAction.classList.add("table-action");
  editAction.onclick = function() { showEditForm(weightId) };

  let deleteAction = document.createElement("img");
  deleteAction.src = "images/trash-2-svgrepo-com.svg"
  deleteAction.classList.add("table-action");
  deleteAction.onclick = function() { deleteWeightAction(weightId) };
  
  actionData.appendChild(editAction);
  actionData.appendChild(deleteAction);
  return actionData;
}

function getTableRow(entry) {
  let tableRow = document.createElement("tr");

  let dateData = document.createElement("td");
  dateData.innerText = entry.date;

  let weightData = document.createElement("td");
  weightData.innerText = parseFloat(entry.userWeight).toFixed(2);

  let descriptionData = document.createElement("td");
  descriptionData.innerText = entry.description !== undefined ? entry.description : "";

  tableRow.appendChild(dateData);
  tableRow.appendChild(weightData);
  tableRow.appendChild(descriptionData);
  tableRow.appendChild(getRowActions(entry.id));

  return tableRow;
}

function createTable(weightData) {
  const table = document.getElementById("weight-table");
  removeChildElements(table);
  const tableHeaderRow = getTableHeaderElement();
  table.appendChild(tableHeaderRow);
  
  weightData.results.reverse().forEach(entry => {
    table.appendChild(getTableRow(entry));
  });
}

window.addEventListener("load", async () => {
  handleSignOut();
  handleAddWeightFormSubmit();
  handleUpdateWeightFormSubmit();
  createChart((await data).results.reverse());
  createTable(await data);
});
 
// Do this straight away
if (!isLoggedIn()) {
  clearCredentialsAndRedirectToLogin();
}

// FIX DATE 
let dateTo = new Date();
let dateFrom = new Date();
dateFrom.setDate(dateTo.getDate() - 30)

let data = getWeightData(
  limit=30, 
  offset=0, 
  dateFrom=dateFrom.toISOString().split('T')[0], 
  dateTo=dateTo.toISOString().split('T')[0]
);
