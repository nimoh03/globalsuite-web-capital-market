/** @type {import('next').NextConfig} */

const securityHeaders = [
    {
        key: 'X-DNS-Prefetch-Control',
        value: 'on',
    },
    {
        key: 'Strict-Transport-Security',
        value: 'max-age=63072000; includeSubDomains; preload',
    },
    {
        key: 'X-XSS-Protection',
        value: '1; mode=block'
    },
    {
        key: 'X-Frame-Options',
        value: 'SAMEORIGIN'
    },
    {
        key: 'X-Content-Type-Options',
        value: 'nosniff'
    },
    {
        key: 'Referrer-Policy',
        value: 'origin-when-cross-origin'
    }
];

const nextConfig = {

    compiler: {
        // removeConsole: process.env.NODE_ENV === 'development' ? false : { exclude: ['error'] },
    },
    images: {
        domains: [
            'localhost',
            'localhost:3000',
            'web.globalsuite.ng',
            'www.web.globalsuite.ng',
        ]
    },
    async headers() { //renable after fixing login
        return [
            {
                // Apply these headers to all routes in your application.
                source: '/:path*',
                headers: securityHeaders,
            },
        ];
    },
    async redirects() { //renable after fixing login
        return [
            {
                source: '/',
                destination: '/login',
                permanent: false,
            }
        ];
    },
}

module.exports = nextConfig
