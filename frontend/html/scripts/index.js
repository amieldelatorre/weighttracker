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

  const headers = ["Date", "Weight", "Description"];
  headers.forEach(header => {
    let childTableHeader = document.createElement("th");
    childTableHeader.innerText = header;
    tableHeaderElementRow.appendChild(childTableHeader);
  });

  return tableHeaderElementRow;
}

function getTableRow(entry) {
  let tableRow = document.createElement("tr");

  let dateData = document.createElement("td");
  dateData.innerText = entry.date;

  let weightData = document.createElement("td");
  weightData.innerText = parseFloat(entry.userWeight).toFixed(2);

  let descriptionData = document.createElement("td");
  descriptionData.innerText = document.description !== undefined ? document.description : "";

  tableRow.appendChild(dateData);
  tableRow.appendChild(weightData);
  tableRow.appendChild(descriptionData);

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
