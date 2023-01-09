import React, { ReactNode } from "react";
import { Container } from "reactstrap";
import { NavMenu } from "./nav-menu";
import { Footer } from "./footer";

export function Layout(props: Parameters<React.FC>[0] & { children: ReactNode }): ReturnType<React.FC> {
    return (
        <div>
            <NavMenu />
            <Container>{props.children}</Container>
            <Footer />
        </div>
    );
}
