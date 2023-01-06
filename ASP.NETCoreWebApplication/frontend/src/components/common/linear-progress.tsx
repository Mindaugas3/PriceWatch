import React, { Component } from "react";
import { LinearProgress } from "@mui/material";

export class ColoredLinearProgress extends Component {
    render() {
        return <LinearProgress color={"secondary"} />;
    }
}
