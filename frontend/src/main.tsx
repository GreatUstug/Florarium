import {StrictMode} from 'react';
import {createRoot} from 'react-dom/client';
import {RouterProvider} from 'react-router';
import {clearStack, connectLogger, context} from '@reatom/core';
import {reatomContext} from '@reatom/react';
import './styles/classical.css';
import {router} from './router';

clearStack();

const rootFrame = context.start();
if (import.meta.env.DEV) {
    rootFrame.run(connectLogger);
}

const root = document.getElementById('root');
if (!root) {
    throw new Error('нет #root в index.html');
}

createRoot(root).render(
    <StrictMode>
        <reatomContext.Provider value={rootFrame}>
            <RouterProvider router={router}/>
        </reatomContext.Provider>
    </StrictMode>,
);
