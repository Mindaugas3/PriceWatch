import React, { Component } from 'react';
import { withStyles } from '@mui/material/styles';
import { LinearProgress } from '@mui/material';

const styles = () => ({
    colorPrimary: {
        backgroundColor: '#00695C',
    },
    barColorPrimary: {
        backgroundColor: 'purple',
    }
});

class ColoredLinearProgress extends Component {
    render() {
        return <LinearProgress color={"secondary"} />;
    }
}

export default ColoredLinearProgress;