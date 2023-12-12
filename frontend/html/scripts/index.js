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


async function getWeightData(limit=100, offset=0, dateFrom, dateTo) {
  let params = new URLSearchParams({
    limit: limit,
    offset: offset
  });
  if (typeof dateFrom !== "undefined" ) {
    params.append("dateFrom", dateFrom)
  }
  if (typeof dateTo !== "undefined" ) {
    params.append("dateTo", dateTo)
  }

  let url = `${API_URL}/Weight?` + params;
  weightData = await fetch(URL=url, {
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

  return weightData;
}


async function createChart(weightData) {
  new Chart(
    document.getElementById('chart').getContext('2d'),
    {
      type: 'line',
      data: {
        labels: weightData.map(row => row.date),
        datasets: [
          {
            label: `Weight the past ${limit} days`,
            data: weightData.map(row => row.userWeight),
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
          }
        ]
      }
    }
  );
}

window.addEventListener("load", async () => {
  handleSignOut();
  handleAddWeightFormSubmit();
  createChart((await data).results.reverse());
});

let dateTo = new Date();
let dateFrom = new Date();
dateFrom.setDate(dateTo.getDate() - 30)

let data = getWeightData(
  limit=30, 
  offset=0, 
  dateFrom=dateFrom.toISOString().split('T')[0], 
  dateTo=dateTo.toISOString().split('T')[0]
);
 

// Do this straight away
checkLoggedIn();
