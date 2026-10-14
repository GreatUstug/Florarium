import type {RouteObject} from 'react-router';
import {AppLayout} from './layout/AppLayout';
import {ScreenStub} from './pages/ScreenStub';
import {NotFound} from './pages/NotFound';
import {screens} from './screens';

const routes: RouteObject[] = [
    {
        element: <AppLayout/>,
        children: [
            ...screens.map((screen) => ({
                path: screen.path,
                element: <ScreenStub screen={screen}/>,
            })),
            {path: '*', element: <NotFound/>},
        ],
    },
];

export {routes};
