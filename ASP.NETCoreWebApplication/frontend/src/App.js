import React, { Component } from "react";
import { Route, Routes } from "react-router";
import { Home } from "./components/Home";
import { Counter } from "./components/Counter";
import { Helmet } from "react-helmet";

import "./custom.css";
import Housing from "./pages/Housing";
import Items from "./pages/Items";

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <React.Fragment>
                <Helmet>
                    <script src="https://kit.fontawesome.com/d8b2026170.js" crossorigin="anonymous"></script>
                </Helmet>
                <Routes>
                    <Route exact path="/" element={<Home />} />
                    <Route path="/counter" element={<Counter />} />
                    <Route path="/Housing" element={<Housing />} />
                    <Route path={"/Items"} element={<Items />} />
                </Routes>
            </React.Fragment>
        );
    }
}
