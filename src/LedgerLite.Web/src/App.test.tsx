import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { App } from './App';
vi.stubGlobal('fetch', vi.fn(()=>Promise.reject(new Error('offline'))));
describe('App',()=>{it('shows the finance overview',()=>{render(<App/>);expect(screen.getByText('Cash balance')).toBeInTheDocument();expect(screen.getByText('Northstar Studio')).toBeInTheDocument()})});

