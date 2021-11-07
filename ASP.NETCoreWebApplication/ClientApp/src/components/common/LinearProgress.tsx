import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import { LinearProgress } from '@material-ui/core';

// interface IProgressClasses {
//     colorPrimary: string,
//     barColorPrimary: string
// }
//
// interface IPlaceholderProps {
//     classes: IProgressClasses
// }

const styles = () => ({
    colorPrimary: {
        backgroundColor: '#00695C',
    },
    barColorPrimary: {
        backgroundColor: 'purple',
    }
});

class ColoredLinearProgress extends Component{
    render() {
        return <LinearProgress color={"secondary"} />;
    }
}

export default ColoredLinearProgress;