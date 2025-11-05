let container = document.getElementById("network");

var dataset = {
    nodes: new vis.DataSet(),
    edges: new vis.DataSet(),
};

const changeTracker = [];

dataset.nodes.forEach(saturateNode);
dataset.edges.forEach(saturateEdge);

const options = {
    interaction: {
        hover: true,
        selectConnectedEdges: false,
        hoverConnectedEdges: false
    },
    manipulation: {
        enabled: true,
        initiallyActive: true,
        addNode: addNode,
        editNode: editNode,
        deleteEdge: true,
        editEdge: true,
        deleteNode: false,
        addEdge: addEdge
    },
    locales: {
        en: {
            edit: 'Edit',
            del: 'Delete selected',
            back: 'Back',
            addNode: 'Find Cluster',
            addEdge: 'Add Edge',
            editNode: 'Edit Node',
            editEdge: 'Edit Edge',
            addDescription: 'Click in an empty space to place a new cluster.',
            edgeDescription: 'Click on a node and drag the edge to another node to connect them.',
            editEdgeDescription: 'Click on the control points and drag them to a node to connect to it.',
            createEdgeError: 'Cannot link edges to a cluster.',
            deleteClusterError: 'Clusters cannot be deleted.',
            editClusterError: 'Clusters cannot be edited.'
        }
    }
};

var network = new vis.Network(container, dataset, options);

var editNodeModal = new bootstrap.Modal('#editNodeModal');
var findClusterModal = new bootstrap.Modal('#findClusterModal');
var saveNetworkModal = new bootstrap.Modal('#saveNetworkModal');

const toastEl = document.getElementById('success-toast');

const toast = new bootstrap.Toast(toastEl, {
    autohide: true,
    delay: 3000
});

function getAntiForgeryToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

function redirectToSignIn() {
    window.location.replace('/MicrosoftIdentity/Account/SignIn');
}

$("#editNodeForm").on("submit", function (event) {
    event.preventDefault();

    let nodeId = $("#selectedNode").text();
    let node = dataset.nodes.get(nodeId);

    let reassign = $("#reassignCheckbox").is(':checked');
    let hardLink = $("#hardLinkCheckbox").is(':checked');

    if (reassign) {
        let toClusterId = $("#newCluster").val();
        reassignNode(node, toClusterId);
    }

    if(node.hardLink != hardLink) {
        changeTracker.push(`Hardlink ${hardLink ? 'added to' : 'removed from'} ${node.id} (${node.group})`);
    }

    node.hardLink = hardLink;

    saturateNode(node);

    editNodeModal.hide();
});

$("#editNodeModal").on('hidden.bs.modal', event => {
    // Reset modal state
    $("#editNodeForm").trigger('reset');
    $("#reassignCheckbox").trigger('change');
});

$("#emptyClusterCheckbox").on('change', event => {
    $("#findClusterSection").attr('hidden', event.currentTarget.checked);
    $("#findClusterInput").attr('required', !event.currentTarget.checked);
    $("#findClusterButton").attr('hidden', event.currentTarget.checked);
    $("#randomClusterButton").attr('hidden', !event.currentTarget.checked);
});

document.getElementById("findClusterForm").addEventListener("submit", async event => {
    event.preventDefault();

    let action = event.submitter.formAction;

    let group = $("#findClusterInput").val().toUpperCase();

    if (clusterExists(group)) {
        alert(`Cluster '${group}' has already been added to the network.`);
        return;
    }

    $('#randomClusterButton').prop('disabled', true);
    $('#findClusterButton').prop('disabled', true);
    $('#cancelClusterButton').prop('disabled', true);

    fetch(action + '?upci=' + group, { method: "GET" })
        .then(async response => {

        if (response.status == 401) {
            redirectToSignIn();
            return;
        }

        if (!response.ok) {
            let error = await response.text();
            alert('Status code: ' + response.status + '. Error: ' + error);
            return;
        }

        const cluster = await response.json();

        cluster.nodes.forEach(node => saturateNode(node, updateNetwork = false));
        cluster.edges.forEach(edge => saturateEdge(edge, updateNetwork = false));

        dataset.nodes.add(cluster.nodes);
        dataset.edges.add(cluster.edges);

        changeTracker.push(`Cluster ${cluster.upci} added to network`);

        findClusterModal.hide();
    })
    .catch(error => {
        console.error(error);
        alert(`Something went wrong. Error: ${error}`)
    })
    .finally(() => {
        $('#randomClusterButton').prop('disabled', false);
        $('#findClusterButton').prop('disabled', false);
        $('#cancelClusterButton').prop('disabled', false);
    });
});

$("#findClusterModal").on('hidden.bs.modal', event => {
    // Reset modal state
    $("#findClusterForm").trigger('reset');
    $("#emptyClusterCheckbox").trigger('change');
    $("#reassignCheckbox").prop('disabled', getClusters().length < 2);
});

$("#reassignCheckbox").on('change', event => {
    $("#reassignSection").attr('hidden', !event.currentTarget.checked);
})

$("#saveNetworkButton").on("click", function() {
    $("#changeTracker").html(`<ul class="list-group list-group-flush list-group-numbered">${changeTracker.map(change => `<li class="list-group-item">${change}</li>`).join('')}</ul>`);
    saveNetworkModal.show();
});

document.getElementById("saveNetwork").addEventListener("submit", async event => {
    event.preventDefault();

    let action = event.target.action;

    let body = {
        clusters: getNetwork()
    };

    $('#saveNetworkBtn').prop('disabled', true);
    $('#saveNetworkCancelBtn').prop('disabled', true);

    fetch(action, {
        method: "POST",
        headers: {
            "RequestVerificationToken": getAntiForgeryToken(),
            "Content-Type": "application/json",
        },
        body: JSON.stringify(body)
    })
    .then(async response => {

        if (response.status == 401) {
            redirectToSignIn();
            return;
        }

        if (!response.ok) {
            let error = await response.text();
            alert('Status code: ' + response.status + '. Error: ' + error);
            return;
        }

        saveNetworkModal.hide();
        toast.show();
    })
    .catch(error => {
        console.error(error);
        alert(`Something went wrong. Error: ${error}`)
    })
    .finally(() => {
        $('#saveNetworkBtn').prop('disabled', false);
        $('#saveNetworkCancelBtn').prop('disabled', false);
    });
});

function addEdge(node, callback) {
    let clusterFrom = dataset.nodes.get(node.from);
    var clusterTo = dataset.nodes.get(node.to);

    // Prevent adding edges to self
    if (node.from == node.to) {
        callback(null);
        return;
    }

    // Prevent adding edges between nodes if either one does not belong to a cluster
    if (clusterFrom == undefined || clusterTo == undefined) {
        callback(null);
        alert("Edges can only be added between nodes that belong to a cluster.");
        return;
    }

    // Prevent adding edges between different clusters
    if (clusterFrom.group !== clusterTo.group) {
        callback(null);
        alert("Edges can only be added between nodes that belong to the same cluster.");
        return;
    }

    // Prevent adding edges that already exist
    if (edgeExists(node.from, node.to)) {
        callback(null);
        alert("An edge already exists between these two nodes.");
        return;
    }

    node.probability = 1.0;
    saturateEdge(node);

    changeTracker.push(`Edge added from ${node.from} to ${node.to}`);

    callback(node);
}

function addNode(data, callback) {
    findClusterModal.show();
    callback(null);
}

function deleteEdge(data, callback) {
    console.log(data);
    callback(data);
}

function clusterExists(groupId) {
    return dataset.nodes.get().some(node => node.group == groupId);
}

function edgeExists(fromNodeId, toNodeId) {
    let x = dataset.edges.get();
    return x.find(edge => (edge.from === fromNodeId && edge.to === toNodeId) || (edge.from === toNodeId && edge.to === fromNodeId)) != undefined;
}

function editNode(node, callback) {
    if (node.type === "cluster") {
        alert("You cannot edit an empty cluster. Reassign nodes to it instead.");
        callback(null);
        return;
    }

    editNodeModal.show();

    $("#selectedNode").text(node.id);
    $("#oldCluster").text(node.group);

    // Populate the select with all clusters except the currently selected one
    let select = $("#newCluster");
    select.empty();
    getClusters().filter(cluster => cluster !== node.group).forEach(cluster => {
        select.append(new Option(cluster, cluster));
    });

    callback(node);
}

function getEdges(groupId) {
    let nodes = getNodes(groupId).map(n => n.id);
    return dataset.edges.get().filter(e => nodes.includes(e.from));
}

function getEdgeColor(probability) {
    if (probability == 1.0) {
        return "darkgreen";
    } else if (probability > 0.75) {
        return "green";
    } else if (probability > 0.5) {
        return "orange";
    } else {
        return "red";
    }
}

function getNodeColor(groupId) {
    // --- Deterministic hue from string ---
    let hash = 0;
    for (let i = 0; i < groupId.length; i++) {
        hash = groupId.charCodeAt(i) + ((hash << 5) - hash);
    }

    const hue = Math.abs(hash) % 360;

    return {
        background: `hsl(${hue}, 60%, 80%)`,
        border: `hsl(${hue}, 60%, 50%)`,
        highlight: {
            background: `hsl(${hue}, 70%, 60%)`,
            border: `hsl(${hue}, 70%, 40%)`
        },
        hover: {
            background: `hsl(${hue}, 65%, 75%)`,
            border: `hsl(${hue}, 65%, 45%)`
        }
    };
}

function getNodes(groupId) {
    return dataset.nodes.get().filter(node => node.group == groupId);
}

function getClusters() {
    let clusters = new Set();
    dataset.nodes.get().forEach(node => {
        clusters.add(node.group);
    });
    return Array.from(clusters).sort();
}

function getNetwork() {
    let upcis = getClusters();

    clusters = [];

    upcis.forEach(upci => clusters.push({
        UPCI: upci,
        Nodes: getNodes(upci).filter(n => n.type == "node"),
        Edges: getEdges(upci)
    }));

    return clusters;
}

function getTooltipContent(html) {
    const container = document.createElement("div");
    container.innerHTML = html;
    return container;
}

function isCluster(nodeId) {
    let node = dataset.nodes.get(nodeId);
    return node && node.type === "cluster";
}

function saturateNode(node, updateNetwork = true) {
    node.label = node.id;

    if (node.hardLink == true) {
        node.label += '*';
        node.shape = "box";
        node.shapeProperties = { borderRadius: 1 };
    }
    else {
        node.shape = 'ellipse';
        node.shapeProperties = { borderRadius: 6 };
    }

    // A cluster itself is a node, but we want to visually distinguish it
    if (node.type === "cluster") {
        node.shape = "hexagon";
        node.size = 75;
    }

    if (node.type === "node") {
        const cros = () => node.metadata?.croNumbers.map(cro => `<li>${cro}</li>`).join('');
        const pncs = () => node.metadata?.pncNumbers.map(pnc => `<li>${pnc}</li>`).join('');
        const noms = () => node.metadata?.nomisNumbers.map(nom => `<li>${nom}</li>`).join('');

        node.title = getTooltipContent(`
            UPCI: ${node.group}<br/>
            Source: ${node.source}<br/>
            Hard Link: ${node.hardLink === true ? "Yes" : "No"}<br/>
            Active: ${node.metadata?.isActive === true ? "Yes" : "No"}<br/>
            <hr>
            First Name: ${node.metadata?.firstName ?? ""}<br/>
            Middle Name: ${node.metadata?.middleName ?? ""}<br/>
            Last Name: ${node.metadata?.lastName ?? ""}<br/>
            Gender: ${node.metadata?.gender ?? ""}<br/>
            DOB: ${node.metadata?.dateOfBirth ?? ""}<br/>
            <hr>
            <details open>
                <summary>CROs:</summary>
                <ul>
                    ${cros()}
                <ul>
            </details>
            <details open>
                <summary>PNCs:</summary>
                <ul>
                    ${pncs()}
                <ul>
            </details>
            <details open>
                <summary>NOMS:</summary>
                <ul>
                    ${noms()}
                <ul>
            </details>
        `);
    }

    node.color = getNodeColor(node.group);

    if (updateNetwork) {
        dataset.nodes.update(node);
    }
}

function saturateEdge(edge, updateNetwork = true) {
    edge.color = getEdgeColor(edge.probability);

    // A little weird, but this prevents rounding
    edge.label = (Math.floor(edge.probability * 100) / 100).toFixed(2).toString();

    edge.title = edge.probability.toString();
    edge.dashes = edge.probability == 1;

    if (updateNetwork) {
        dataset.edges.update(edge);
    }
}

function reassignNode(node, toClusterId) {
    // Remove all edges connected to this node
    let edges = dataset.edges.get({ filter: (edge) => edge.from === node.id || edge.to === node.id });
    edges.forEach(edge => dataset.edges.remove(edge.id));

    const fromClusterId = node.id;

    // Destroy target cluster
    if (isCluster(toClusterId)) {
        dataset.nodes.remove(toClusterId);
    }

    // If the node is the only member of its current cluster,
    // add an empty cluster to the graph.
    // This way, we don't stop tracking the cluster.
    if (getNodes(node.group).length == 1) {
        let cluster = {
            id: node.group,
            group: node.group,
            type: 'cluster'
        };

        saturateNode(cluster, updateNetwork = false);

        dataset.nodes.add(cluster);
    }

    // Create new edges to all nodes in the target cluster
    let nodes = dataset.nodes.get().filter(n => n.group === toClusterId);
    nodes.forEach(n => {
        let edge = { from: node.id, to: n.id, probability: 1 };
        dataset.edges.add(edge);
        saturateEdge(edge);
    });

    changeTracker.push(`${node.id} reassigned from ${fromClusterId} to ${toClusterId}`);

    // Move the node to target group
    node.group = toClusterId;
    saturateNode(node);
}