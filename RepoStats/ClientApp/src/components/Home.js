import React, { useState } from "react";
import { InputGroup, InputGroupAddon, Input, Button, Spinner } from "reactstrap";
import "../App.css";
import "./Home.css";

const RepoState = {
  UNKNOWN: 0,
  READY: 1,
  PROCESSING: 2,
  FAILED: 3,
  ERROR: 4,
}

export function Home() {
  const [fieldValue, setFieldValue] = useState("");
  const [isBusy, setIsBusy] = useState(false);
  const [repoState, setRepoState] = useState(RepoState.UNKNOWN);
  const [iframeSource, setIframeSource] = useState("");

  const startQuery = async () => {
    setIsBusy(true);

    try {
      const response = await fetch(`query/Report?repoUrl=${fieldValue}`, {
        headers: {
          "Content-Type": "application/json",
          "Accept": "application/json"
         }
      });

      console.log(response);
      let iframeUrl = "";
      if (response.status === 200) {
        iframeUrl = await response.json();

        setRepoState(RepoState.READY);
      } else if (response.status === 202) {
        setRepoState(RepoState.PROCESSING);
      } else if (response.status === 403 || response.status === 400) {
        setRepoState(RepoState.FAILED);
      } else if (response.status === 500) {
        setRepoState(RepoState.ERROR);
      } else {
        setRepoState(RepoState.UNKNOWN);
      }

      setFieldValue("");
      setIframeSource(iframeUrl);
    } catch (e) {
      console.log(e);
    } finally {
      setIsBusy(false);
    }
  }

  return (
    <div class="flex-container">
      <div>
        <InputGroup style={{ width: "50%", margin: "auto" }}>
          <Input
            value={fieldValue}
            onChange={(e) => setFieldValue(e.target.value)}
            disabled={isBusy}
            placeholder="Enter: owner/repository"
          />
          <InputGroupAddon addonType="prepend" style={{ marginLeft: "10px" }}>
            <Button color="primary" disabled={isBusy} onClick={startQuery}>Query</Button>
          </InputGroupAddon>
        </InputGroup>
      </div>
      {isBusy &&
        <div style={{ width: "auto", margin: "auto", display: "flex", flexFlow: "column" }}>
          <p style={{ testAlign: "center", fontSize: "xx-large" }}>Processing request ...</p>
          <Spinner style={{ width: "5rem", height: "5rem", margin: "auto", fontSize: "xxx-large" }} />
        </div>
      }
      {repoState === RepoState.PROCESSING && !isBusy &&
      <div style={{ width: "auto", margin: "auto", display: "flex", flexFlow: "column" }}>
        <p style={{ testAlign: "center", fontSize: "xx-large" }}>Report is being processed.</p>
        <p style={{ testAlign: "center", fontSize: "xx-large" }}>Come back after a moment ...</p>
        <Spinner style={{ width: "5rem", height: "5rem", margin: "auto", fontSize: "xxx-large" }} />
      </div>
      }
      {repoState === RepoState.FAILED && !isBusy &&
      <div style={{ width: "auto", margin: "auto", display: "flex", flexFlow: "column" }}>
        <p style={{ testAlign: "center", color: "red", fontSize: "xx-large" }}>Failed to process given repository.</p>
        <p style={{ testAlign: "center", color: "red", fontSize: "xx-large" }}>Please restart your query.</p>
      </div>
      }
      {repoState === RepoState.ERROR && !isBusy &&
      <div style={{ width: "auto", margin: "auto", display: "flex", flexFlow: "column" }}>
        <p style={{ testAlign: "center", color: "red", fontSize: "xx-large" }}>An error has occured while processing your request.</p>
      </div>
      }
      {!isBusy && repoState === RepoState.READY &&
        <iframe
          src={iframeSource}
          class="report-iframe"
          title="PowerBIReport"
          width="calc(100% - 50px)"
          height="calc(100% - 50px)"
          frameBorder="0"
          allowFullScreen={true}>
        </iframe>
      }
    </div>
  );
}
